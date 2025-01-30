using UnityEngine;

namespace Board.Pieces
{
    public class Knight : Piece
    {
        [Header("Knight")]
        public Vector2Int[] Moves;

        protected override void CalculateMovesAndCapturesInternal()
        {
            MoveSquares.Clear();
            CaptureSquares.Clear();

            // Calculate Moves and Captures
            foreach (Vector2Int offset in Moves)
            {
                var square = gameManager.GetSquareRel(pieceColor, currentSquare, offset);

                if (square == gameManager.NullSquare)
                {
                    continue;
                }

                if (!square.HasPiece())
                {
                    MoveSquares.Add(square);
                }
                else if (square.GetPieceColor() != pieceColor)
                {
                    CaptureSquares.Add(square);
                }
            }
        }
    }
}