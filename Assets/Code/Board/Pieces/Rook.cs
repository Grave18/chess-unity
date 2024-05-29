using UnityEngine;

namespace Board.Pieces
{
    public class Rook : Piece
    {
        [Header("Rook")]
        public Vector2Int[] MoveVectors;

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

            foreach (Vector2Int direction in MoveVectors)
            {
                var currentDir = direction;
                while (true)
                {
                    var possibleSquare = gameManager.GetSquare(currentSquare, currentDir);
                    // Advance for one square
                    currentDir += direction;
                    Debug.Log(possibleSquare.name);

                    if (possibleSquare == gameManager.NullSquare)
                    {
                        break;
                    }

                    // Move or eat
                    if (square == possibleSquare && possibleSquare.GetPieceColor() != pieceColor)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}