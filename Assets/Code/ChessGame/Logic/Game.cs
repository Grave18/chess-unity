using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChessGame.ChessBoard;
using ChessGame.ChessBoard.Pieces;
using ChessGame.Logic.GameStates;
using ChessGame.Logic.MovesBuffer;
using ChessGame.Logic.Players;
using MainCamera;
using Ui.MainMenu;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace ChessGame.Logic
{
    public class Game : SingletonBehaviour<Game>
    {
        [field: Header("References")]
        [field:SerializeField] public Competitors Competitors { get; set; }
        public Board Board { get; private set; }
        public UciBuffer UciBuffer { get; private set; }

        [Header("Settings")]
        [SerializeField] private int rule50Count = 50;
        [SerializeField] private int ruleThreefoldCount = 3;

        public PieceColor CurrentTurnColor { get; private set; } = PieceColor.White;
        public PieceColor PreviousTurnColor => CurrentTurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        public CheckType CheckType { get; private set; } = CheckType.None;
        public string CheckDescription { get; private set; }

        public ISelectable Selected { get; private set; }

        public AttackLinesList AttackLines { get; } = new();
        public HashSet<Square> UnderAttackSquares { get; set; } = new();

        private GameState _state;
        private GameState _previousState;
        private PieceColor _startingColor;
        private Competitors _competitors;
        private CameraController _cameraController;
        private PieceColor _winnerColor;

        public event UnityAction OnWarmup;
        public event UnityAction OnStart;
        public event UnityAction<PieceColor> OnStartColor;
        public event UnityAction OnEnd;
        public event UnityAction OnEndMove;
        public event UnityAction<PieceColor> OnEndMoveColor;
        public event UnityAction OnPlay;
        public event UnityAction OnPause;

        public void FireWarmup() => OnWarmup?.Invoke();
        public void FireStart()
        {
            OnStart?.Invoke();
            OnStartColor?.Invoke(CurrentTurnColor);
        }
        public void FireEnd() => OnEnd?.Invoke();
        public void FireEndMove()
        {
            OnEndMove?.Invoke();
            OnEndMoveColor?.Invoke(CurrentTurnColor);
        }
        public void FirePlay() => OnPlay?.Invoke();
        public void FirePause() => OnPause?.Invoke();

        // End game
        public bool IsCheck => CheckType is CheckType.Check or CheckType.DoubleCheck;
        public bool IsCheckMate => CheckType == CheckType.CheckMate;
        public bool IsDraw => CheckType == CheckType.Draw;
        public bool IsWinnerWhite => _winnerColor == PieceColor.White;
        public bool IsWinnerBlack => _winnerColor == PieceColor.Black;
        public bool IsGameOver() => CheckType is not CheckType.None and not CheckType.Check and not CheckType.DoubleCheck;
        private bool IsTimeOut() => CheckType is CheckType.TimeOutWhite or CheckType.TimeOutBlack;

        /// Is it turn of current player
        public bool IsMyTurn() => CurrentTurnColor == GameSettingsContainer.Instance.GameSettings.PlayerColor;
        public bool IsWhiteTurn() => CurrentTurnColor == PieceColor.White;
        public bool IsBlackTurn() => CurrentTurnColor == PieceColor.Black;

        // Getters
        public HashSet<Piece> WhitePieces => Board.WhitePieces;
        public HashSet<Piece> BlackPieces => Board.BlackPieces;
        public HashSet<Piece> CurrentTurnPieces => CurrentTurnColor == PieceColor.White ? Board.WhitePieces : Board.BlackPieces;
        public HashSet<Piece> PrevTurnPieces => CurrentTurnColor == PieceColor.Black ? Board.WhitePieces : Board.BlackPieces;
        public IEnumerable<Square> Squares => Board.Squares;
        public Square NullSquare => Board.NullSquare;

        public void Init(Board board, Competitors competitors, CameraController cameraController, UciBuffer commandUciBuffer, PieceColor color)
        {
            Board = board;
            _competitors = competitors;
            _cameraController = cameraController;
            UciBuffer = commandUciBuffer;
            _startingColor = color;
        }

        public void RestartGame()
        {
            _competitors.Restart();
            Board.Build();
            StartGame();
        }

        public void StartGame()
        {
            bool isCameraSet = false;
            _cameraController.RotateToStartPosition(() => isCameraSet = true);

            ResetGameState();
            PreformCalculations();
            StartCoroutine(StartGameRoutine());
            SetState(new WarmUpState(this));

            return;

            IEnumerator StartGameRoutine()
            {
                yield return new WaitUntil(() => isCameraSet);

                SetState(new IdleState(this));
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

        public void SetState(GameState state, string nextState = "None", bool isSetPreviousState = true)
        {
            _previousState = isSetPreviousState
                ? _state
                : null;

            _state?.Exit(nextState);
            _state = state;
            _state?.Enter();
        }

        public void SetPreviousState()
        {
            if (_previousState != null)
            {
                SetState(_previousState, _previousState.Name);
                _previousState = null;
            }
            else
            {
                SetState(new IdleState(this), "Idle");
                Debug.Log("Go to default Idle state");
            }
        }

        public string GetStateName()
        {
            return _state?.Name ?? "No State";
        }

        public void ChangeTurn()
        {
            CurrentTurnColor = CurrentTurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            Competitors.SwapCurrentPlayer();
        }

        private void Update()
        {
            _state?.Update();
        }

        public void Move(string uci)
        {
            _state?.Move(uci);
        }

        public void Undo()
        {
            _state?.Undo();
        }

        public void Redo()
        {
            _state?.Redo();
        }

        public void Play()
        {
            _state?.Play();
        }

        public void Pause()
        {
            _state?.Pause();
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

            SetState(new EndGameState(this));
        }

        public void Resign()
        {
            CheckType = CheckType.Resign;
            CheckDescription = $"{CurrentTurnColor} player resigned";
            _winnerColor = PreviousTurnColor;

            SetState(new EndGameState(this));
        }

        public void DrawByAgreement()
        {
            Draw("Draw by agreement");
        }

        private void Draw(string description)
        {
            CheckType = CheckType.Draw;
            _winnerColor = PieceColor.None;
            CheckDescription = description;

            SetState(new EndGameState(this));
        }

        public void CheckCheckmate()
        {
            if (CheckType is not CheckType.CheckMate)
            {
                return;
            }

            _winnerColor = PreviousTurnColor;
            SetState(new EndGameState(this));
        }

        /// Calculate under attack squares, check type, moves and captures
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

        private bool IsThreefoldRule()
        {
            bool isThreefold = UciBuffer.ThreefoldRepetitionCount == ruleThreefoldCount;

            return isThreefold;
        }

        private bool IsRule50()
        {
            return UciBuffer.Rule50Count == rule50Count * 2;
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