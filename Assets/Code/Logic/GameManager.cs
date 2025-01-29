using System.Collections.Generic;
using System.Linq;
using Board;
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
        [SerializeField] private Transform squaresTransform;
        [SerializeField] private Transform whitePiecesTransform;
        [SerializeField] private Transform blackPiecesTransform;


        [Header("Settings")]
        [FormerlySerializedAs("CurrentTurn")]
        [SerializeField]private PieceColor currentTurn = PieceColor.White;
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
        private CheckType _checkType;

        // Getters
        public CheckType CheckType => _checkType;
        public PieceColor CurrentTurn => currentTurn;

        /// <summary>
        /// Pieces who is threatening the king
        /// </summary>
        public List<Piece> Attackers => attackers;
        public List<Square> AttackLine => attackLine;

        public Square NullSquare => nullSquare;
        public bool IsAutoChange => isAutoChange;
        public Square[] Squares => squares;
        public Square[] UnderAttackSquares => underAttackSquares;

        private void Start()
        {
            CalculateUnderAttackSquares();
        }

        /// <summary>
        ///  Calculate Under Attack Squares of opposite color
        /// </summary>
        public void CalculateUnderAttackSquares()
        {
            var currentTurnPieces = currentTurn == PieceColor.White ? blackPieces : whitePieces;
            var underAttackSet = new HashSet<Square>();

            attackers.Clear();

            foreach (Piece piece in currentTurnPieces)
            {
                piece.CalculateUnderAttackSquares();

                // Try to add attackers
                if (TryCalculateCheck(piece.UnderAttackSquares))
                {
                    attackers.Add(piece);

                    // Fill under attack line
                    foreach (Square square in piece.UnderAttackSquares)
                    {
                        attackLine.Add(square);
                    }
                }

                foreach (var pieceSquare in piece.UnderAttackSquares)
                {
                    underAttackSet.Add(pieceSquare);
                }
            }

            // Infer check type
            _checkType = attackers.Count switch
                {
                    0 => CheckType.None,
                    1 => CheckType.Check,
                    _ => CheckType.DoubleCheck
                };

            underAttackSquares = underAttackSet.ToArray();
        }

        public void SetAutoChange(bool value)
        {
            isAutoChange = value;
        }

        public void SetTurn(PieceColor turn)
        {
            if (turn == PieceColor.None)
            {
                return;
            }

            currentTurn = turn;
            CalculateUnderAttackSquares();
        }

        public void ChangeTurn()
        {
            if (!isAutoChange)
            {
                return;
            }

            // Change turn
            currentTurn = currentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;

            CalculateUnderAttackSquares();
        }

        public bool IsRightTurn(PieceColor pieceColor)
        {
            return pieceColor == currentTurn;
        }

        public void ClearSquares()
        {
            whitePieces = new Piece[] { };
            blackPieces = new Piece[] { };
        }

        public Square GetSquare(PieceColor pieceColor, Square currentSquare, Vector2Int offset)
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

        private bool TryCalculateCheck(IEnumerable<Square> squares)
        {
            foreach (Square square in squares)
            {
                if (square.HasPiece() && square.GetPiece() is King king && king.GetPieceColor() == currentTurn)
                {
                    return true;
                }
            }

            return false;
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
        public void FindAllSections()
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

            currentTurn = (PieceColor)index;
            CalculateUnderAttackSquares();
        }

#endif
    }
}
