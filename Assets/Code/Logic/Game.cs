using System;
using System.Collections.Generic;
using System.Linq;
using ChessBoard;
using ChessBoard.Info;
using ChessBoard.Pieces;
using EditorCools;
using Logic.CommandPattern;
using UnityEngine;

namespace Logic
{
    public class Game : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CommandInvoker commandInvoker;

        [Header("Settings")]
        [SerializeField] private GameState state = GameState.Idle;
        [SerializeField] private PieceColor currentTurnColor = PieceColor.White;
        [SerializeField] private CheckType checkType = CheckType.None;
        [SerializeField] private bool isAutoChange;

        public ISelectable Selected { get; set; }
        public ISelectable Highlighted { get; set; }

        public AttackLinesList AttackLines { get; } = new();
        public HashSet<Square> UnderAttackSquares { get; private set; } = new();

        public event Action OnStartTurn;
        public event Action OnEndTurn;
        public event Action OnStart;
        public event Action OnEnd;
        public event Action OnPlay;
        public event Action OnPause;
        public event Action OnStartThink;
        public event Action OnEndThink;


        private Board _board;
        private PieceColor _timeOutColor = PieceColor.None;

        // Getters
        public CheckType CheckType => checkType;
        public PieceColor CurrentTurnColor => currentTurnColor;
        public PieceColor PreviousTurnColor => currentTurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        public GameState State => state;

        public HashSet<Piece> WhitePieces => _board.WhitePieces;
        public HashSet<Piece> BlackPieces => _board.BlackPieces;
        public IEnumerable<Square> Squares => _board.Squares;
        public Square NullSquare => _board.NullSquare;

        public bool IsAutoChange => isAutoChange;

        private HashSet<Piece> CurrentTurnPieces => currentTurnColor == PieceColor.White ? _board.WhitePieces : _board.BlackPieces;
        private HashSet<Piece> PrevTurnPieces => currentTurnColor == PieceColor.Black ? _board.WhitePieces : _board.BlackPieces;

        public void Init(Board board, PieceColor turnColor)
        {
            _board = board;
            currentTurnColor = turnColor;
        }

        public void StartGame()
        {
            checkType = CheckType.None;
            state = GameState.Idle;

            _board.Build();
            CalculateEndMove();

            OnEndTurn?.Invoke();
            OnStart?.Invoke();
        }

        public void Play()
        {
            if(state != GameState.Pause)
            {
                return;
            }

            state = GameState.Idle;
            OnPlay?.Invoke();
        }

        public void Pause()
        {
            if(state != GameState.Idle)
            {
                return;
            }

            state = GameState.Pause;
            OnPause?.Invoke();
        }

        public void StartTurn()
        {
            state = GameState.Move;
            OnStartTurn?.Invoke();
        }

        public void EndTurn()
        {
            currentTurnColor = currentTurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            state = GameState.Idle;

            CalculateEndMove();
            OnEndTurn?.Invoke();

            if(IsGameOver())
            {
                OnEnd?.Invoke();
            }
        }

        public void StartThink()
        {
            state = GameState.Think;
            OnStartThink?.Invoke();
        }

        public void EndThink()
        {
            state = GameState.Idle;
            OnEndThink?.Invoke();
        }

        public PieceColor GetWinner()
        {
            if (checkType == CheckType.CheckMate)
            {
                return currentTurnColor ==  PieceColor.White ? PieceColor.Black : PieceColor.White;
            }

            if (checkType == CheckType.TimeOut)
            {
                return _timeOutColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            }

            if (checkType == CheckType.Stalemate)
            {
                return PieceColor.None;
            }

            return PieceColor.None;
        }

        public void SetTimeOut(PieceColor pieceColor)
        {
            checkType = CheckType.TimeOut;
            _timeOutColor = pieceColor;

            OnEnd?.Invoke();
        }

        public bool IsGameOver()
        {
            return checkType is CheckType.CheckMate or CheckType.Stalemate or CheckType.TimeOut;
        }

        /// Retrieves the En Passant information for the last command if applicable
        public EnPassantInfo GetEnPassantInfo()
        {
            return commandInvoker.GetEnPassantInfo();
        }

        public bool IsRightTurn(PieceColor pieceColor)
        {
            return pieceColor == currentTurnColor;
        }

        /// Get section relative to current piece color
        public Square GetSquareRel(PieceColor pieceColor, Square currentSquare, Vector2Int offset)
        {
            return _board.GetSquareRel(pieceColor, currentSquare, offset);
        }

        /// Get section relative to absolute position (white side)
        public Square GetSquareAbs(Square currentSquare, Vector2Int offset)
        {
            return _board.GetSquareAbs(currentSquare, offset);
        }

        /// Calculations for all turns
        private void CalculateEndMove()
        {
            UnderAttackSquares = GetUnderAttackSquares(PrevTurnPieces);
            checkType = CalculateCheck(PrevTurnPieces);

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

            return AttackLines.GetCheckCount() switch
            {
                0 => CheckType.None,
                1 => CheckType.Check,
                _ => CheckType.DoubleCheck
            };

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
            if (currentTurnPieces.Any(piece => piece.MoveSquares.Count  > 0
                || piece.CaptureSquares.Count > 0))
            {
                return;
            }

            checkType = checkType switch
                {
                    CheckType.None => CheckType.Stalemate,
                    CheckType.Check or CheckType.DoubleCheck => CheckType.CheckMate,
                    _ => checkType
                };
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        public void SetAutoChange(bool value)
        {
            isAutoChange = value;
        }

        public void SetTurn(int index)
        {
            if (index < 0 || index > 1)
            {
                return;
            }

            currentTurnColor = (PieceColor)index;
            state = GameState.Idle;

            CalculateEndMove();
            OnEndTurn?.Invoke();
        }
#endif
    }
}