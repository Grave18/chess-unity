using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChessGame.ChessBoard;
using ChessGame.ChessBoard.Pieces;
using ChessGame.Logic.GameStates;
using ChessGame.Logic.MovesBuffer;
using ChessGame.Logic.Players;
using MainCamera;
using UnityEngine;
using UnityEngine.Events;

namespace ChessGame.Logic
{
    public class Game : MonoBehaviour
    {
        [field: Header("References")]
        [field:SerializeField] public Competitors Competitors { get; set; }
        public Board Board { get; private set; }
        public UciBuffer UciBuffer { get; private set; }

        public PieceColor CurrentTurnColor { get; private set; } = PieceColor.White;
        public PieceColor PreviousTurnColor => CurrentTurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        public CheckType CheckType { get; set; } = CheckType.None;

        public ISelectable Selected { get; private set; }

        public AttackLinesList AttackLines { get; } = new();
        public HashSet<Square> UnderAttackSquares { get; set; } = new();

        public event UnityAction OnStart;
        public event UnityAction<PieceColor> OnStartColor;
        public event UnityAction OnEnd;
        public event UnityAction OnEndMove;
        public event UnityAction<PieceColor> OnEndMoveColor;
        public event UnityAction OnPlay;
        public event UnityAction OnPause;

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

        private GameState _state;
        private GameState _previousState;

        // Getters
        public HashSet<Piece> WhitePieces => Board.WhitePieces;
        public HashSet<Piece> BlackPieces => Board.BlackPieces;
        public HashSet<Piece> CurrentTurnPieces => CurrentTurnColor == PieceColor.White ? Board.WhitePieces : Board.BlackPieces;
        public HashSet<Piece> PrevTurnPieces => CurrentTurnColor == PieceColor.Black ? Board.WhitePieces : Board.BlackPieces;
        public IEnumerable<Square> Squares => Board.Squares;
        public Square NullSquare => Board.NullSquare;

        private PieceColor _startingColor;
        private Competitors _competitors;
        private CameraController _cameraController;

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

            StartCoroutine(StartGameRoutine());

            return;

            IEnumerator StartGameRoutine()
            {
                yield return new WaitUntil(() => isCameraSet);

                ResetGameState();
                PreformCalculations();
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

        public void SetState(GameState state, string nextState = "None")
        {
            _previousState = _state;
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

        public PieceColor GetWinner()
        {
            if (CheckType == CheckType.CheckMate)
            {
                return CurrentTurnColor ==  PieceColor.White ? PieceColor.Black : PieceColor.White;
            }

            if (CheckType == CheckType.TimeOutWhite)
            {
                return PieceColor.Black;
            }

            if (CheckType == CheckType.TimeOutBlack)
            {
                return PieceColor.White;
            }

            if (CheckType == CheckType.Stalemate)
            {
                return PieceColor.None;
            }

            return PieceColor.None;
        }

        public void SetTimeOut(PieceColor pieceColor)
        {
            if (pieceColor == PieceColor.White)
            {
                CheckType = CheckType.TimeOutWhite;
            }
            else if (pieceColor == PieceColor.Black)
            {
                CheckType = CheckType.TimeOutBlack;
            }

            FireEnd();
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

        public string GetStateName()
        {
            return _state?.Name ?? "No State";
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

        public bool IsWhiteTurn()
        {
            return CurrentTurnColor == PieceColor.White;
        }

        public bool IsBlackTurn()
        {
            return CurrentTurnColor == PieceColor.Black;
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

            CalculateCheckMateOrStalemate(CurrentTurnPieces);
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

        private void CalculateCheckMateOrStalemate(HashSet<Piece> currentTurnPieces)
        {
            // If all pieces have no moves
            if (IsAnyPieceHasMove(currentTurnPieces))
            {
                return;
            }

            CheckType = CheckType switch
            {
                CheckType.None => CheckType.Stalemate,
                CheckType.Check or CheckType.DoubleCheck => CheckType.CheckMate,
                _ => CheckType,
            };
        }

        private static bool IsAnyPieceHasMove(HashSet<Piece> currentTurnPieces)
        {
            return currentTurnPieces.Any(piece => piece.MoveSquares.Count  > 0
                                                  || piece.CaptureSquares.Count > 0);
        }
    }
}