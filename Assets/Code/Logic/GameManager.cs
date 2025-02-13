using System;
using System.Collections.Generic;
using System.Linq;
using Board;
using Board.Builder;
using Board.Pieces;
using EditorCools;
using UnityEngine;

namespace Logic
{
    public class GameManager : MonoBehaviour
    {
        private const int Width = 8;
        private const int Height = 8;

        [Header("References")]
        [SerializeField] private BoardBuilder boardBuilder;
        [SerializeField] private CommandManager commandManager;
        [SerializeField] private Transform squaresTransform;
        [SerializeField] private Transform whitePiecesTransform;
        [SerializeField] private Transform blackPiecesTransform;

        [Header("Settings")]
        [SerializeField] private GameState gameState = GameState.Idle;
        [SerializeField] private PieceColor currentTurnColor = PieceColor.White;
        [SerializeField] private CheckType checkType = CheckType.None;
        [SerializeField] private bool isAutoChange;

        [Header("Arrays")]
        [SerializeField] private Square[] squares;
        [SerializeField] private Square nullSquare;

        [Header("Selections")]
        public ISelectable Selected;
        public ISelectable Highlighted;

        public HashSet<Piece> WhitePieces { get; } = new();
        public HashSet<Piece> BlackPieces { get; } = new();
        private HashSet<Piece> CurrentTurnPieces => currentTurnColor == PieceColor.White ? WhitePieces : BlackPieces;
        private HashSet<Piece> PrevTurnPieces => currentTurnColor == PieceColor.Black ? WhitePieces : BlackPieces;

        public AttackLinesList AttackLines { get; } = new();
        public HashSet<Square> UnderAttackSquares { get; private set; } = new();

        public event Action<PieceColor, CheckType> OnTurnChanged;
        public event Action OnRestart;

        // Getters
        public CheckType CheckType => checkType;
        public PieceColor CurrentTurnColor => currentTurnColor;
        public GameState GameState => gameState;

        public Square NullSquare => nullSquare;
        public bool IsAutoChange => isAutoChange;
        public Square[] Squares => squares;

        private void Start()
        {
            FindAllSquares();
            Restart();
        }

        public void ClearPieces()
        {
            WhitePieces.Clear();
            BlackPieces.Clear();
        }

        public void RemovePiece(Piece piece)
        {
            if (piece.GetPieceColor() == PieceColor.White)
            {
                WhitePieces.Remove(piece);
            }
            else
            {
                BlackPieces.Remove(piece);
            }
        }

        public void AddPiece(Piece piece)
        {
            if (piece.GetPieceColor() == PieceColor.White)
            {
                WhitePieces.Add(piece);
            }
            else
            {
                BlackPieces.Add(piece);
            }
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

        public bool IsEndgame()
        {
            return checkType is CheckType.CheckMate or CheckType.Stalemate;
        }

        [Button(space: 10f)]
        public void Restart()
        {
            checkType = CheckType.None;
            gameState = GameState.Idle;
            boardBuilder.BuildBoard(out currentTurnColor);
            FindAllPieces();
            EndMoveCalculations();

            OnTurnChanged?.Invoke(currentTurnColor, checkType);
            OnRestart?.Invoke();
        }

        public void StartTurn()
        {
            gameState = GameState.Move;
        }

        public void EndTurn()
        {
            // Change turn
            currentTurnColor = currentTurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            gameState = GameState.Idle;

            EndMoveCalculations();
            OnTurnChanged?.Invoke(currentTurnColor, checkType);
        }

        // Calculations for all turns. Need to call every turn change
        private void EndMoveCalculations()
        {
            HashSet<Piece> currentTurnPieces = CurrentTurnPieces;
            HashSet<Piece> prevTurnPieces = PrevTurnPieces;

            UnderAttackSquares = GetUnderAttackSquares(prevTurnPieces);
            checkType = CalculateCheck(prevTurnPieces);

            foreach (Piece piece in currentTurnPieces)
            {
                piece.CalculateMovesAndCaptures();
                piece.CalculateConstrains();
            }

            CalculateCheckMateOrStalemate(currentTurnPieces);
        }

        private static HashSet<Square> GetUnderAttackSquares(HashSet<Piece> currentTurnPieces)
        {
            var underAttackSet = new HashSet<Square>();
            foreach (Piece piece in currentTurnPieces)
            {
                piece.CalculateMovesAndCaptures();

                foreach (Square underAttackSquare in piece.MoveSquares)
                {
                    underAttackSet.Add(underAttackSquare);
                }
                foreach (Square defendSquare in piece.DefendSquares)
                {
                    underAttackSet.Add(defendSquare);
                }
            }

            return underAttackSet;
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
                foreach ((Square square, Piece _) in piece.CaptureSquares)
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

        /// <summary>
        /// Get last moved piece from command buffer
        /// </summary>
        /// <returns> Last moved piece </returns>
        public Piece GetLastMovedPiece()
        {
            return commandManager.GetLastMovedPiece();
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
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                // Return Null section (last in array)
                return NullSquare;
            }

            return squares[y + x * Width];
        }

        // Editor tools
        public void SetAutoChange(bool value)
        {
            isAutoChange = value;
        }

        [Button(space: 10f)]
        [ContextMenu("Find All Pieces")]
        public void FindAllPieces()
        {

            foreach (Transform whitePieceTransform in whitePiecesTransform)
            {
                WhitePieces.Add(whitePieceTransform.GetComponent<Piece>());
            }

            foreach (Transform blackPieceTransform in blackPiecesTransform)
            {
                BlackPieces.Add(blackPieceTransform.GetComponent<Piece>());
            }
        }

        [Button(space: 10f)]
        [ContextMenu("Find All Squares")]
        public void FindAllSquares()
        {
            var squaresTemp = new List<Square>();

            foreach (Transform squareTransform in squaresTransform)
            {
                squaresTemp.Add(squareTransform.GetComponent<Square>());
            }

            squares = squaresTemp.ToArray();

            SetupSquares();
        }

        /// Fill squares with coordinates
        private void SetupSquares()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int index = y + x * Width;

                    var square = squares[index];
                    square.X = x;
                    square.Y = y;

                    square.File = $"{(char)(x + 'a')}";
                    square.Rank = $"{y + 1}";
                }
            }
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD

        /// <summary>
        /// For debug select tool
        /// </summary>
        /// <param name="index"></param>
        public void SetTurn(int index)
        {
            if (index < 0 || index > 1)
            {
                return;
            }

            currentTurnColor = (PieceColor)index;
            EndMoveCalculations();
            OnTurnChanged?.Invoke(currentTurnColor, checkType);
        }

#endif
    }
}