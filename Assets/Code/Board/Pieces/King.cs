using UnityEngine;

namespace Board.Pieces
{
    public class King : Piece
    {
        [Header("King")]
        public Vector2Int[] Moves;

        public override void CalculateUnderAttackSquares()
        {
            UnderAttackSquares.Clear();

            foreach (Vector2Int offset in Moves)
            {
                var underAttackSquare = gameManager.GetSquare(pieceColor, currentSquare, offset);

                // Has Piece
                if (underAttackSquare.HasPiece())
                {
                    if (underAttackSquare.GetPieceColor() == pieceColor)
                    {
                        continue;
                    }

                    UnderAttackSquares.Add(underAttackSquare);
                    continue;
                }

                // Off board
                if (underAttackSquare == gameManager.NullSquare)
                {
                    continue;
                }

                // Empty Square
                UnderAttackSquares.Add(underAttackSquare);
            }
        }

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

            CalculateUnderAttackSquares();

            foreach (var underAttackSquare in UnderAttackSquares)
            {
                if (underAttackSquare == square)
                {
                    return true;
                }
            }

            return false;
        }
    }
}