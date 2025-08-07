using System.Collections.Generic;
using System.Linq;
using ChessBoard;
using ChessBoard.Pieces;
using Logic.GameStates;
using Logic.MovesBuffer;
using Logic.Players;
using MainCamera;
using Settings;
using UnityEngine;
using UnityEngine.Events;

namespace Logic
{
    public class Game : MonoBehaviour
    {
        public MachineManager Machine { get; private set; }
        public Competitors Competitors { get; private set; }
        public Board Board { get; private set; }
        public UciBuffer UciBuffer { get; private set; }

        public PieceColor CurrentTurnColor { get; private set; } = PieceColor.White;
        public PieceColor PreviousTurnColor => CurrentTurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        public CheckType CheckType { get; private set; } = CheckType.None;
        public string CheckDescription { get; private set; }
        public HashSet<Square> UnderAttackSquares { get; private set; } = new();
        public AttackLinesList AttackLines { get; } = new();

        public ISelectable Selected { get; private set; }

        [Header("Settings")]
        [SerializeField] private int rule50Count = 50;
        [SerializeField] private int ruleThreefoldCount = 3;

        private PieceColor _startingColor;
        private CameraController _cameraController;
        private PieceColor _winnerColor;
        private GameSettingsContainer _gameSettingsContainer;

        // Board shortcuts
        public HashSet<Piece> WhitePieces => Board.WhitePieces;
        public HashSet<Piece> BlackPieces => Board.BlackPieces;
        public IEnumerable<Square> Squares => Board.Squares;
        public Square NullSquare => Board.NullSquare;

        // End game
        public bool IsCheck => CheckType is CheckType.Check or CheckType.DoubleCheck;
        public bool IsCheckMate => CheckType == CheckType.CheckMate;
        public bool IsDraw => CheckType == CheckType.Draw;
        public bool IsWinnerWhite => _winnerColor == PieceColor.White;
        public bool IsWinnerBlack => _winnerColor == PieceColor.Black;
        private bool IsTimeOut => CheckType is CheckType.TimeOutWhite or CheckType.TimeOutBlack;
        public bool IsGameOver => CheckType is not CheckType.None and not CheckType.Check and not CheckType.DoubleCheck;

        /// Is it turn of current player
        public bool IsMyTurn => CurrentTurnColor == _gameSettingsContainer.GameSettings.PlayerColor;
        public bool IsWhiteTurn => CurrentTurnColor == PieceColor.White;
        public bool IsBlackTurn => CurrentTurnColor == PieceColor.Black;

        public HashSet<Piece> CurrentTurnPieces => CurrentTurnColor == PieceColor.White ? Board.WhitePieces : Board.BlackPieces;
        public HashSet<Piece> PrevTurnPieces => CurrentTurnColor == PieceColor.Black ? Board.WhitePieces : Board.BlackPieces;

        // Events and invokers
        public event UnityAction OnWarmup;
        public event UnityAction OnStart;
        public event UnityAction<PieceColor> OnStartWithColor;
        public event UnityAction OnEnd;
        public event UnityAction OnEndMove;
        public event UnityAction<PieceColor> OnEndMoveWithColor;
        public event UnityAction OnPlay;
        public event UnityAction OnPause;

        public void FireWarmup() => OnWarmup?.Invoke();
        public void FireStart() { OnStart?.Invoke(); OnStartWithColor?.Invoke(CurrentTurnColor); }
        public void FireEnd() => OnEnd?.Invoke();
        public void FireEndMove() { OnEndMove?.Invoke(); OnEndMoveWithColor?.Invoke(CurrentTurnColor); }
        public void FirePlay() => OnPlay?.Invoke();
        public void FirePause() => OnPause?.Invoke();

        public void Init(Board board, Competitors competitors, CameraController cameraController, UciBuffer commandUciBuffer,
            PieceColor color, GameSettingsContainer gameSettingsContainer, MachineManager machine)
        {
            Board = board;
            Competitors = competitors;
            _cameraController = cameraController;
            UciBuffer = commandUciBuffer;
            _startingColor = color;
            _gameSettingsContainer = gameSettingsContainer;
            Machine = machine;
        }

        public void StartGame()
        {
            _cameraController.RotateToStartPosition(StartGameContinuation);

            ResetGameState();
            PreformCalculations();
            Machine.SetState(new WarmUpState(this));

            return;

            void StartGameContinuation()
            {
                Machine.SetState(new IdleState(this));
                FireStart();
            }
        }

        private void ResetGameState()
        {
            CheckType = CheckType.None;
            CurrentTurnColor = _startingColor;
            Selected = null;
            UciBuffer.Clear();
        }

        public void ChangeTurn()
        {
            CurrentTurnColor = CurrentTurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            Competitors.SwapCurrentPlayer();
        }

        public void SetTimeOut(PieceColor pieceColor)
        {
            if (pieceColor == PieceColor.White)
            {
                CheckType = CheckType.TimeOutWhite;
                _winnerColor = PieceColor.Black;
            }
            else if (pieceColor == PieceColor.Black)
            {
                CheckType = CheckType.TimeOutBlack;
                _winnerColor = PieceColor.White;
            }

            Machine.SetState(new EndGameState(this));
        }

        public void Checkmate()
        {
            _winnerColor = PreviousTurnColor;
            Machine.SetState(new EndGameState(this));
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

            CalculateDraw();
            CalculateCheckMate(CurrentTurnPieces);
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

        private void CalculateDraw()
        {
            if (IsThreefoldRule())
            {
                Draw("By threefold repetition");
            }
            else if (IsRule50())
            {
                Draw("By fifty move rule");
            }
            else if(IsInsufficientPieces())
            {
                // Description was set earlier
                Draw(CheckDescription);
            }
        }

        public void Rematch()
        {
            Competitors.Restart();
            Board.Build();
            StartGame();
        }

        public void Draw(string description)
        {
            CheckType = CheckType.Draw;
            CheckDescription = description;
            _winnerColor = PieceColor.None;

            Machine.SetState(new EndGameState(this));
        }

        public void Resign()
        {
            CheckType = CheckType.Resign;
            CheckDescription = $"{CurrentTurnColor} player resigned";
            _winnerColor = PreviousTurnColor;

            Machine.SetState(new EndGameState(this));
        }

        private bool IsThreefoldRule()
        {
            bool isThreefold = UciBuffer.ThreefoldRepetitionCount == ruleThreefoldCount;

            return isThreefold;
        }

        private bool IsRule50()
        {
            return UciBuffer.HalfMoveClock == rule50Count * 2;
        }

        private bool IsInsufficientPieces()
        {
            bool isInsufficientPieces = IsInsufficientPiecesOneSide(CurrentTurnPieces, PrevTurnPieces)
                          || IsInsufficientPiecesOneSide(PrevTurnPieces, CurrentTurnPieces);

            return isInsufficientPieces;
        }

        private bool IsInsufficientPiecesOneSide(HashSet<Piece> thisSide, HashSet<Piece> otherSide)
        {
            bool oneKingThisSide = thisSide.Count == 1 && thisSide.First() is King;
            bool isDraw = oneKingThisSide && IsInsufficientFiguresInOtherSide(otherSide);

            return isDraw;
        }

        /// K-K; K-KN; K-KNN; K-KB
        private bool IsInsufficientFiguresInOtherSide(HashSet<Piece> otherSide)
        {
            bool hasKing = otherSide.Count > 0 && otherSide.Any(p => p is King);

            bool oneKing = otherSide.Count == 1 && hasKing;
            if (oneKing)
            {
                CheckDescription = "Insufficient figures. Both sides has only one king";
                return true;
            }

            bool kingAndBishop = otherSide.Count == 2 && hasKing && otherSide.Any(p => p is Bishop);
            if (kingAndBishop)
            {
                CheckDescription = "Insufficient figures. One side has one king and other king and bishop";
                return true;
            }

            bool kingAndKnight = otherSide.Count == 2 && hasKing && otherSide.Any(p => p is Knight);
            if (kingAndKnight)
            {
                CheckDescription = "Insufficient figures. One side has one king and other king and knight";
                return true;
            }

            bool kingAnd2Knights = otherSide.Count == 3 && hasKing && otherSide.OfType<Knight>().Count() == 2;
            if (kingAnd2Knights)
            {
                CheckDescription = "Insufficient figures. One side has one king and other king and 2 knights";
                return true;
            }

            return false;
        }

        private void CalculateCheckMate(HashSet<Piece> currentTurnPieces)
        {
            // If all pieces have no moves exit early
            if (IsAnyPieceHasMove(currentTurnPieces))
            {
                return;
            }

            switch (CheckType)
            {
                case CheckType.None:
                    CheckType = CheckType.Draw;
                    CheckDescription = "Stalemate. Players have no moves";
                    break;
                case CheckType.Check or CheckType.DoubleCheck:
                    CheckType = CheckType.CheckMate;
                    break;
                default:
                    CheckType = CheckType;
                    break;
            }
        }

        private static bool IsAnyPieceHasMove(HashSet<Piece> currentTurnPieces)
        {
            return currentTurnPieces.Any(piece => piece.MoveSquares.Count  > 0
                                                  || piece.CaptureSquares.Count > 0);
        }

        // Board shortcuts
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
    }
}