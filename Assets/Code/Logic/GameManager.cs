using System;
using System.Collections.Generic;
using System.Linq;
using Board;
using Board.Builder;
using Board.Pieces;
using EditorCools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Logic
{
    public class GameManager : MonoBehaviour
    {
        private const int Width = 8;
        private const int Height = 8;

        [Header("References")]
        [SerializeField] private BoardBuilder boardBuilder;
        [SerializeField] private Transform squaresTransform;
        [SerializeField] private Transform whitePiecesTransform;
        [SerializeField] private Transform blackPiecesTransform;

        [Header("Settings")]
        [SerializeField] private PieceColor currentTurnColor = PieceColor.White;
        [SerializeField] private PieceColor defaultTurnColor = PieceColor.White;
        [SerializeField] private CheckType checkType = CheckType.None;
        [SerializeField] private bool isAutoChange;

        [Header("Arrays")]
        [FormerlySerializedAs("Squares")]
        [SerializeField] private Square[] squares;
        [SerializeField] private Piece[] whitePieces;
        [SerializeField] private Piece[] blackPieces;
        [SerializeField] private Square[] underAttackSquares;
        [SerializeField] private List<Piece> attackers;
        [SerializeField] private List<Square> attackLine;
        [SerializeField] private Square nullSquare;

        public ISelectable Selected;
        public ISelectable Highlighted;

        public event Action<PieceColor, CheckType> OnTurnChanged;

        // Getters
        public CheckType CheckType => checkType;
        public PieceColor CurrentTurnColor => currentTurnColor;

        /// <summary>
        /// Pieces who is threatening the king
        /// </summary>
        public List<Piece> Attackers => attackers;
        public List<Square> AttackLine => attackLine;

        public Square NullSquare => nullSquare;
        public bool IsAutoChange => isAutoChange;
        public Square[] Squares => squares;
        public Square[] UnderAttackSquares => underAttackSquares;

        public Piece[] WhitePieces => whitePieces;
        public Piece[] BlackPieces => blackPieces;

        private void Start()
        {
            Restart();
        }

        public void ClearSquares()
        {
            whitePieces = new Piece[] { };
            blackPieces = new Piece[] { };
        }

        [Button(space: 10f)]
        public void Restart()
        {
            currentTurnColor = defaultTurnColor;
            checkType = CheckType.None;
            boardBuilder.BuildBoard();
            FindAllPieces();
            FindAllSquares();

            CalculateUnderAttackSquares();
            OnTurnChanged?.Invoke(currentTurnColor, checkType);
        }

        /// <summary>
        ///  Calculate Under Attack Squares of opposite color
        /// </summary>
        private void CalculateUnderAttackSquares()
        {
            Piece[] currentTurnPieces = currentTurnColor == PieceColor.White ? whitePieces : blackPieces;
            Piece[] prevTurnPieces = currentTurnColor == PieceColor.Black ? whitePieces : blackPieces;

            underAttackSquares = GetUnderAttackSquares(prevTurnPieces);
            checkType = CalculateCheck(prevTurnPieces);

            foreach (Piece piece in currentTurnPieces)
            {
                piece.CalculateMovesAndCaptures();
                piece.CalculateConstrains();
            }
        }

        private static Square[] GetUnderAttackSquares(Piece[] currentTurnPieces)
        {
            var underAttackSet = new HashSet<Square>();
            foreach (Piece piece in currentTurnPieces)
            {
                piece.CalculateMovesAndCaptures();

                foreach (Square underAttackSquare in piece.MoveSquares)
                {
                    underAttackSet.Add(underAttackSquare);
                }
            }
            return underAttackSet.ToArray();
        }

        private CheckType CalculateCheck(Piece[] pieces)
        {
            // 2) Calculate check
            attackers.Clear();
            attackLine.Clear();
            foreach (Piece piece in pieces)
            {
                // Try to add attackers
                if (IsPieceMakeCheck(piece))
                {
                    attackers.Add(piece);

                    // Fill under attack line
                    if (piece is LongRange longRange)
                    {
                        attackLine.AddRange(longRange.AttackLineSquares);
                    }
                }
            }

            return attackers.Count switch
            {
                0 => CheckType.None,
                1 => CheckType.Check,
                _ => CheckType.DoubleCheck
            };

            bool IsPieceMakeCheck(Piece piece)
            {
                foreach (Square square in piece.CaptureSquares)
                {
                    if (square.HasPiece() && square.GetPiece() is King king && king.GetPieceColor() != piece.GetPieceColor())
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public void ChangeTurn()
        {
            if (!isAutoChange)
            {
                return;
            }

            // Change turn
            currentTurnColor = currentTurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;

            CalculateUnderAttackSquares();
            OnTurnChanged?.Invoke(currentTurnColor, checkType);
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
            var whitePiecesTemp = new List<Piece>();
            var blackPiecesTemp = new List<Piece>();

            foreach (Transform whitePieceTransform in whitePiecesTransform)
            {
                whitePiecesTemp.Add(whitePieceTransform.GetComponent<Piece>());
            }

            foreach (Transform blackPieceTransform in blackPiecesTransform)
            {
                blackPiecesTemp.Add(blackPieceTransform.GetComponent<Piece>());
            }

            whitePieces = whitePiecesTemp.ToArray();
            blackPieces = blackPiecesTemp.ToArray();
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
            CalculateUnderAttackSquares();
            OnTurnChanged?.Invoke(currentTurnColor, checkType);
        }

#endif
    }
}
