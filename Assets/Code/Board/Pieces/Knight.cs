using UnityEngine;
using UnityEngine.Serialization;

namespace Board.Pieces
{
    public class Knight : Piece
    {
        [Header("Knight")]
        [FormerlySerializedAs("Moves")]
        [SerializeField] private Vector2Int[] moves;

        protected override void CalculateMovesAndCapturesInternal()
        {
            foreach (Vector2Int offset in moves)
            {
                Square square = gameManager.GetSquareRel(pieceColor, currentSquare, offset);

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