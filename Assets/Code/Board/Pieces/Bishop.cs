using UnityEngine;

namespace Board.Pieces
{
    public class Bishop : Piece
    {
        [Header("Bishop")]
        public Vector2Int[] Moves;

        protected override bool CanEatAt(Square square)
        {
            return CanMoveTo(square);
        }

        protected override bool CanMoveTo(Square square)
        {
            if (square.HasPiece() && square.GetPieceColor() == pieceColor)
            {
                return false;
            }

            foreach (Vector2Int offset in Moves)
            {
                var possibleSection = gameManager.GetSquare(currentSquare, offset);
                Debug.Log(possibleSection.name);

                if (possibleSection == square)
                {
                    return true;
                }
            }

            return false;
        }
    }
}