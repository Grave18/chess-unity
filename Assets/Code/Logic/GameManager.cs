using System.Collections.Generic;
using System.Linq;
using Board;
using Board.Pieces;
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
        public PieceColor CurrentTurn = PieceColor.White;
        [SerializeField] private bool isAutoChange;

        [Header("Arrays")]
        [FormerlySerializedAs("Squares")]
        [SerializeField] private Square[] squares;
        [SerializeField] private Piece[] whitePieces;
        [SerializeField] private Piece[] blackPieces;
        [SerializeField] private Square[] underAttackSquares;

        public ISelectable Selected;
        public ISelectable Highlighted;

        // Getters
        public Square NullSquare => squares[^1];
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
            var pieces = CurrentTurn == PieceColor.White ? blackPieces : whitePieces;
            var underAttackSet = new HashSet<Square>();

            foreach (var piece in pieces)
            {
                piece.CalculateUnderAttackSquares();

                foreach (var pieceSquare in piece.UnderAttackSquares)
                {
                    underAttackSet.Add(pieceSquare);
                }
            }

            underAttackSquares = underAttackSet.ToArray();
        }

        public void SetAutoChange(bool value)
        {
            isAutoChange = value;
        }

        public void SetCurrentTurn(int index)
        {
            if (index < 0 || index > 1)
            {
                return;
            }

            CurrentTurn = (PieceColor)index;
        }

        public void ChangeTurn()
        {
            if (!isAutoChange)
            {
                return;
            }

            CurrentTurn = CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;

            CalculateUnderAttackSquares();
        }

        public bool IsRightTurn(PieceColor pieceColor)
        {
            return pieceColor == CurrentTurn;
        }

        public void SetCurrentTurn(PieceColor turn)
        {
            if (turn == PieceColor.None)
            {
                return;
            }

            CurrentTurn = turn;
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

        [ContextMenu("Find All Sections")]
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

                    var square = this.squares[index];
                    square.X = x;
                    square.Y = y;

                    square.File = $"{(char)(x + 'a')}";
                    square.Rank = $"{y + 1}";

                }
            }
        }
    }
}
