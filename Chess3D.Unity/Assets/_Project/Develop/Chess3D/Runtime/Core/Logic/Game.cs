using System.Collections.Generic;
using System.Linq;
using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.ChessBoard.Pieces;
using Chess3D.Runtime.Core.Logic.GameStates;
using Chess3D.Runtime.Core.Logic.MenuStates;
using Chess3D.Runtime.Core.Logic.MovesBuffer;
using Chess3D.Runtime.Core.Logic.Players;
using Chess3D.Runtime.Core.MainCamera;
using Cysharp.Threading.Tasks;
using PurrNet.StateMachine;
using UnityEngine;
using UnityEngine.Events;

namespace Chess3D.Runtime.Core.Logic
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private StateNode warmupState;
        [SerializeField] private StateNode idleState;

        public IGameStateMachine GameStateMachine { get; private set; }
        public MenuStateMachine MenuStateMachine { get; private set; }
        public Competitors Competitors { get; private set; }
        public Board Board { get; private set; }
        public UciBuffer UciBuffer { get; private set; }

        public PieceColor CurrentTurnColor { get; private set; } = PieceColor.White;
        public PieceColor PreviousTurnColor => CurrentTurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        public CheckType CheckType { get; private set; } = CheckType.None;
        public string EndGameDescription { get; private set; }
        public HashSet<Square> UnderAttackSquares { get; private set; } = new();
        public AttackLinesList AttackLines { get; } = new();
        private PieceColor _winnerColor;

        // Draw Rules
        private int _fiftyMoveRuleCount;
        private int _threefoldRepetitionCount;

        private PieceColor _startingColor;
        private CameraController _cameraController;
        private GameSettingsContainer _gameSettingsContainer;

        // End game
        public bool IsCheck => CheckType is CheckType.Check or CheckType.DoubleCheck;
        public bool IsCheckmate => CheckType is CheckType.Checkmate;
        public bool IsDraw => CheckType == CheckType.Draw;
        public bool IsTimeOut => CheckType is CheckType.TimeOut;
        public bool IsResign => CheckType is CheckType.Resign;
        public bool IsEndGame => IsCheckmate || IsDraw || IsTimeOut || IsResign;
        public bool IsWinnerWhite => _winnerColor == PieceColor.White;
        public bool IsWinnerBlack => _winnerColor == PieceColor.Black;

        /// Is it turn of current player
        public bool IsMyTurn => CurrentTurnColor == _gameSettingsContainer.GameSettings.PlayerColor;
        public bool IsWhiteTurn => CurrentTurnColor == PieceColor.White;
        public bool IsBlackTurn => CurrentTurnColor == PieceColor.Black;

        // Selection
        public ISelectable Selected { get; private set; }

        public HashSet<Piece> CurrentTurnPieces => CurrentTurnColor == PieceColor.White ? Board.WhitePieces : Board.BlackPieces;
        public HashSet<Piece> PrevTurnPieces => CurrentTurnColor == PieceColor.Black ? Board.WhitePieces : Board.BlackPieces;

        // Events and invokers
        public event UnityAction OnWarmup;
        public event UnityAction OnStart;
        public event UnityAction OnIdle;
        public event UnityAction<PieceColor> OnStartWithColor;
        public event UnityAction OnEnd;
        public event UnityAction OnEndMove;
        public event UnityAction<PieceColor> OnEndMoveWithColor;
        public event UnityAction OnPlay;
        public event UnityAction OnPause;

        public void FireWarmup() => OnWarmup?.Invoke();
        public void FireStart() { OnStart?.Invoke(); OnStartWithColor?.Invoke(CurrentTurnColor); }
        public void FireIdle() => OnIdle?.Invoke();
        public void FireEnd() => OnEnd?.Invoke();
        public void FireEndMove() { OnEndMove?.Invoke(); OnEndMoveWithColor?.Invoke(CurrentTurnColor); }
        public void FirePlay() => OnPlay?.Invoke();
        public void FirePause() => OnPause?.Invoke();

        public void Init(Board board, Competitors competitors, CameraController cameraController, UciBuffer commandUciBuffer,
            PieceColor color, GameSettingsContainer gameSettingsContainer, IGameStateMachine gameStateMachine, MenuStateMachine menuStateMachine)
        {
            Board = board;
            Competitors = competitors;
            _cameraController = cameraController;
            UciBuffer = commandUciBuffer;
            _startingColor = color;
            _gameSettingsContainer = gameSettingsContainer;
            GameStateMachine = gameStateMachine;
            MenuStateMachine = menuStateMachine;

            _fiftyMoveRuleCount = _gameSettingsContainer.FiftyMoveRuleCount;
            _threefoldRepetitionCount = _gameSettingsContainer.ThreefoldRepetitionCount;
        }

        public async UniTask StartGame()
        {
            ResetGameState();
            PreformCalculations();
            await UniTask.WaitUntil(() => GameStateMachine.State is not WarmUpState);
        }

        private void ResetGameState()
        {
            CheckType = CheckType.None;
            CurrentTurnColor = _startingColor;
            Selected = null;
            UciBuffer.Clear();
            GameStateMachine.ResetState();
            MenuStateMachine.ResetState();
        }

        public void ChangeTurn()
        {
            CurrentTurnColor = CurrentTurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            Competitors.SwapCurrentPlayer();
        }

        public void Rematch()
        {
            Competitors.Restart();
            Board.Build();
            StartGame().Forget();
        }

        public void TimeOutSetup(PieceColor pieceColor)
        {
            if (pieceColor == PieceColor.White)
            {
                CheckType = CheckType.TimeOut;
                _winnerColor = PieceColor.Black;
                EndGameDescription = "White is timeout";
            }
            else if (pieceColor == PieceColor.Black)
            {
                CheckType = CheckType.TimeOut;
                _winnerColor = PieceColor.White;
                EndGameDescription = "Black is timeout";
            }
        }

        public void CheckmateSetup()
        {
            CheckType = CheckType.Checkmate;
            _winnerColor = PreviousTurnColor;

            string winnerString = _winnerColor == PieceColor.White ? "White" : "Black";
            EndGameDescription = $"{winnerString} winning the game";
        }

        public void DrawSetup(string description)
        {
            CheckType = CheckType.Draw;
            EndGameDescription = description;
            _winnerColor = PieceColor.None;
        }

        public void ResignSetup()
        {
            CheckType = CheckType.Resign;
            EndGameDescription = $"{CurrentTurnColor} player resigned";
            _winnerColor = PreviousTurnColor;
        }

        /// Calculate under attack squares, check type, moves, captures, draw
        public void PreformCalculations()
        {
            UnderAttackSquares = GetUnderAttackSquares(PrevTurnPieces);
            CheckType = CalculateCheck(PrevTurnPieces);

            foreach (Piece piece in CurrentTurnPieces)
            {
                piece.CalculateMovesAndCaptures();
                piece.CalculateConstrains();
            }

            CalculateEndGame(CurrentTurnPieces);
        }

        private static HashSet<Square> GetUnderAttackSquares(HashSet<Piece> pieces)
        {
            var underAttackSquares = new HashSet<Square>();
            foreach (Piece piece in pieces)
            {
                piece.CalculateMovesAndCaptures();
                FillUnderAttackSquaresForPiece(piece, underAttackSquares);
            }

            return underAttackSquares;
        }

        private static void FillUnderAttackSquaresForPiece(Piece piece, HashSet<Square> underAttackSquares)
        {
            // Pawn's under attack
            if (piece is Pawn pawn)
            {
                var attackSquares = new List<Square>(pawn.UnderAttackSquares);
                foreach (Square underAttackSquare in attackSquares)
                {
                    underAttackSquares.Add(underAttackSquare);
                }
            }
            // Other piece's under attack
            else
            {
                var moveSquares = new List<Square>(piece.MoveSquares.Keys);
                foreach (Square moveSquare in moveSquares)
                {
                    underAttackSquares.Add(moveSquare);
                }
            }

            // All pieces defends
            foreach (Square defendSquare in piece.DefendSquares)
            {
                underAttackSquares.Add(defendSquare);
            }
        }

        private CheckType CalculateCheck(HashSet<Piece> pieces)
        {
            AttackLines.Clear();

            foreach (Piece piece in pieces)
            {
                // Fill under attack line
                bool isCheck = IsPieceMakeCheck(piece);
                if (piece is LongRange longRange)
                {
                    if (!longRange.HasAttackLine) continue;

                    var attackLine = new AttackLine(piece, isCheck, longRange.AttackLineSquares, longRange.SquareBehindKing);
                    AttackLines.Add(attackLine);
                }
                else
                {
                    if (!isCheck) continue;

                    var attackLine = new AttackLine(piece, true);
                    AttackLines.Add(attackLine);
                }
            }

            CheckType check = AttackLines.GetCheckCount() switch
            {
                0 => CheckType.None,
                1 => CheckType.Check,
                _ => CheckType.DoubleCheck
            };

            return check;

            bool IsPieceMakeCheck(Piece piece)
            {
                foreach ((Square square, _) in piece.CaptureSquares)
                {
                    if (square.HasPiece() && square.GetPiece() is King king &&
                        king.GetPieceColor() != piece.GetPieceColor())
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private void CalculateEndGame(HashSet<Piece> currentTurnPieces)
        {
            if (IsAnyPieceHasMove(currentTurnPieces))
            {
                if (IsThreefoldRule())
                {
                    DrawSetup("By threefold repetition");
                }
                else if (IsRule50())
                {
                    DrawSetup("By fifty move rule");
                }
                else if(IsInsufficientMaterial(out string checkDescription))
                {
                    DrawSetup(checkDescription);
                }
            }
            else
            {
                switch (CheckType)
                {
                    // Stalemate
                    case CheckType.None:
                        DrawSetup("Stalemate. Players have no moves");
                        break;
                    // Checkmate
                    case CheckType.Check or CheckType.DoubleCheck:
                        CheckmateSetup();
                        break;
                }
            }
        }

        private static bool IsAnyPieceHasMove(HashSet<Piece> currentTurnPieces)
        {
            return currentTurnPieces.Any(piece => piece.MoveSquares.Count  > 0
                                                  || piece.CaptureSquares.Count > 0);
        }

        private bool IsThreefoldRule()
        {
            bool isThreefold = UciBuffer.ThreefoldRepetitionCount >= _threefoldRepetitionCount;

            return isThreefold;
        }

        private bool IsRule50()
        {
            return UciBuffer.HalfMoveClock == _fiftyMoveRuleCount * 2;
        }

        private bool IsInsufficientMaterial(out string checkDescription)
        {
            checkDescription = string.Empty;
            bool isInsufficientPieces = IsInsufficientPiecesOneSide(CurrentTurnPieces, PrevTurnPieces, out checkDescription)
                          || IsInsufficientPiecesOneSide(PrevTurnPieces, CurrentTurnPieces, out checkDescription);

            return isInsufficientPieces;
        }

        private static bool IsInsufficientPiecesOneSide(HashSet<Piece> thisSide, HashSet<Piece> otherSide, out string checkDescription)
        {
            checkDescription = string.Empty;
            bool oneKingThisSide = thisSide.Count == 1 && thisSide.First() is King;
            bool isDraw = oneKingThisSide && IsInsufficientFiguresInOtherSide(otherSide, out checkDescription);

            return isDraw;
        }

        /// K-K; K-KN; K-KNN; K-KB
        private static bool IsInsufficientFiguresInOtherSide(HashSet<Piece> otherSide, out string checkDescription)
        {
            checkDescription = string.Empty;
            bool hasKing = otherSide.Count > 0 && otherSide.Any(p => p is King);

            // K vs K
            bool oneKing = otherSide.Count == 1 && hasKing;
            if (oneKing)
            {
                checkDescription = "Insufficient material. Both sides has only one king";
                return true;
            }

            // K vs KB
            bool kingAndBishop = otherSide.Count == 2 && hasKing && otherSide.Any(p => p is Bishop);
            if (kingAndBishop)
            {
                checkDescription = "Insufficient material. One side has one king and other king and bishop";
                return true;
            }

            // K vs KN
            bool kingAndKnight = otherSide.Count == 2 && hasKing && otherSide.Any(p => p is Knight);
            if (kingAndKnight)
            {
                checkDescription = "Insufficient material. One side has one king and other king and knight";
                return true;
            }

            // K vs KNN
            bool kingAnd2Knights = otherSide.Count == 3 && hasKing && otherSide.OfType<Knight>().Count() == 2;
            if (kingAnd2Knights)
            {
                checkDescription = "Insufficient material. One side has one king and other king and 2 knights";
                return true;
            }

            return false;
        }

#region Board shortcuts

        // Board shortcuts
        public HashSet<Piece> WhitePieces => Board.WhitePieces;
        public HashSet<Piece> BlackPieces => Board.BlackPieces;
        public IEnumerable<Square> Squares => Board.Squares;
        public Square NullSquare => Board.NullSquare;

        /// Get section relative to current piece color
        public Square GetSquareRel(PieceColor pieceColor, Square currentSquare, Vector2Int offset)
        {
            return Board.GetSquareRel(pieceColor, currentSquare, offset);
        }

        /// Get section relative to absolute position (white side)
        public Square GetSquareAbs(Square currentSquare, Vector2Int offset)
        {
            return Board.GetSquareAbs(currentSquare, offset);
        }

        // Selection
        public bool CanSelect(ISelectable selectable)
        {
            return selectable.HasPiece() && selectable.GetPieceColor() == CurrentTurnColor;
        }

        public void Select(ISelectable selectable)
        {
            Selected = selectable;
        }

        /// Deselect currently selected piece
        public void Deselect()
        {
            Selected = null;
        }

#endregion
    }
}