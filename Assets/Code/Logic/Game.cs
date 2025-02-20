using System;
using System.Collections.Generic;
using System.Linq;
using ChessBoard;
using ChessBoard.Pieces;
using EditorCools;
using UnityEngine;

namespace Logic
{
    public class Game : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CommandInvoker commandInvoker;
        [SerializeField] private Transform piecesTransform;

        [Header("Settings")]
        [SerializeField] private GameState gameState = GameState.Idle;
        [SerializeField] private PieceColor currentTurnColor = PieceColor.White;
        [SerializeField] private CheckType checkType = CheckType.None;
        [SerializeField] private bool isAutoChange;

        public ISelectable Selected { get; set; }
        public ISelectable Highlighted { get; set; }

        public AttackLinesList AttackLines { get; } = new();
        public HashSet<Square> UnderAttackSquares { get; private set; } = new();

        public event Action OnStartTurn;
        public event Action<PieceColor, CheckType> OnEndTurn;
        public event Action OnRestart;

        private Board _board;

        // Getters
        public CheckType CheckType => checkType;
        public PieceColor CurrentTurnColor => currentTurnColor;
        public PieceColor PreviousTurnColor => currentTurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        public GameState GameState => gameState;

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

            CalculateEndMove();

            OnEndTurn?.Invoke(currentTurnColor, checkType);
            OnRestart?.Invoke();
        }

        [Button(space: 10f)]
        public void Restart()
        {
            checkType = CheckType.None;
            gameState = GameState.Idle;

            _board.Build();
            CalculateEndMove();

            OnEndTurn?.Invoke(currentTurnColor, checkType);
            OnRestart?.Invoke();
        }

        /// <summary>
        /// Set state as move
        /// </summary>
        public void StartTurn()
        {
            gameState = GameState.Move;
            OnStartTurn?.Invoke();
        }

        /// <summary>
        /// Change color, rest state, perform calculations
        /// </summary>
        public void EndTurn()
        {
            currentTurnColor = currentTurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            gameState = GameState.Idle;

            CalculateEndMove();
            OnEndTurn?.Invoke(currentTurnColor, checkType);
        }

        public void AddPiece(Piece piece)
        {
            _board.AddPiece(piece);
        }

        public void RemovePiece(Piece piece)
        {
            _board.RemovePiece(piece);
        }

        public PieceColor GetWinner()
        {
            return currentTurnColor switch
            {
                PieceColor.White when checkType == CheckType.CheckMate => PieceColor.Black,
                PieceColor.Black when checkType == CheckType.CheckMate => PieceColor.White,
                _ => PieceColor.None
            };
        }

        public bool IsGameOver()
        {
            return checkType is CheckType.CheckMate or CheckType.Stalemate;
        }

        /// <summary>
        /// Get last moved piece from command buffer
        /// </summary>
        /// <returns> Last moved piece </returns>
        public Piece GetLastMovedPiece()
        {
            return commandInvoker.GetLastMovedPiece();
        }

        public bool IsRightTurn(PieceColor pieceColor)
        {
            return pieceColor == currentTurnColor;
        }

        /// <summary>
        /// Get section relative to current piece color
        /// </summary>
        /// <param name="pieceColor">Color of the piece</param>
        /// <param name="currentSquare">Current section of the piece</param>
        /// <param name="offset">Offset from current section</param>
        /// <returns>Section at the offset or null if out of bounds</returns>
        public Square GetSquareRel(PieceColor pieceColor, Square currentSquare, Vector2Int offset)
        {
            int x = -1;
            int y = -1;

            if (pieceColor == PieceColor.White)
            {
                x = currentSquare.X + offset.x;
                y = currentSquare.Y + offset.y;
            }
            else if (pieceColor == PieceColor.Black)
            {
                x = currentSquare.X - offset.x;
                y = currentSquare.Y - offset.y;
            }

            // if out of board bounds
            if (x < 0 || x >= Board.Width || y < 0 || y >= Board.Height)
            {
                // Return Null section (last in array)
                return _board.NullSquare;
            }

            return _board.Squares[y + x * Board.Width];
        }

        /// <summary>
        /// Get section relative to absolute position (white side)
        /// </summary>
        /// <param name="currentSquare"> Current square </param>
        /// <param name="offset"> Offset from current square </param>
        /// <returns> Square at the offset or NullSquare if out of bounds </returns>
        public Square GetSquareAbs(Square currentSquare, Vector2Int offset)
        {
            return GetSquareRel(PieceColor.White, currentSquare, offset);
        }

        // Calculations for all turns. Need to call every turn change
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

                foreach (Square moveSquare in piece.MoveSquares)
                {
                    underAttackSquares.Add(moveSquare);
                }
                foreach (Square defendSquare in piece.DefendSquares)
                {
                    underAttackSquares.Add(defendSquare);
                }
            }

            return underAttackSquares;
        }

        private CheckType CalculateCheck(HashSet<Piece> pieces)
        {
            AttackLines.Clear();
            foreach (Piece piece in pieces)
            {
                // Fill under attack line
                if (piece is LongRange longRange)
                {
                    if (!longRange.HasAttackLine) continue;

                    var attackLine = new AttackLine(piece, IsPieceMakeCheck(piece), longRange.AttackLineSquares);
                    AttackLines.Add(attackLine);
                }
                else
                {
                    if (!IsPieceMakeCheck(piece)) continue;

                    var attackLine = new AttackLine(piece, true);
                    AttackLines.Add(attackLine);
                }
            }

            return AttackLines.CountCheck() switch
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
            gameState = GameState.Idle;

            CalculateEndMove();
            OnEndTurn?.Invoke(currentTurnColor, checkType);
        }
#endif
    }
}