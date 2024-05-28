using UnityEngine;

namespace Board.Pieces
{
    public class Knight : Piece
    {
        [Header("Knight")]
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
                var possibleSquare = gameManager.GetSquare(currentSquare, offset);
                Debug.Log(possibleSquare.name);

                if (possibleSquare == square)
                {
                    return true;
                }
            }

            return false;
        }
    }
}