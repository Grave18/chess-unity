using System.Collections.Generic;
using Board;
using UnityEngine;

namespace Logic
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Transform board;

        private const int Width = 8;
        private const int Height = 8;

        public PieceColor CurrentTurn = PieceColor.White;
        [SerializeField] private bool isAutoChange;

        public Square[] Squares;

        public Square NullSquare => Squares[^1];
        public bool IsAutoChange => isAutoChange;

        public void Construct(Square[] squares)
        {
            Squares = squares;
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

        public Square GetSquare(Square currentSquare, Vector2Int offset)
        {
            int x;
            int y;

            if (CurrentTurn == PieceColor.White)
            {
                x = currentSquare.X + offset.x;
                y = currentSquare.Y + offset.y;
            }
            else
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

            return Squares[y + x * Width];
        }

        [ContextMenu("Find All Sections")]
        private void FindAllSections()
        {
            var squares = new List<Square>();

            foreach (Transform squareTransform in board)
            {
                squares.Add(squareTransform.GetComponent<Square>());
            }

            Construct(squares.ToArray());

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int index = y + x * Width;

                    var section = Squares[index];
                    section.X = x;
                    section.Y = y;
                }
            }
        }
    }
}
