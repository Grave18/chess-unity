using UnityEngine;

namespace Board.Pieces
{
    public class Knight : Piece
    {
        [Header("Knight")]
        public Vector2Int[] Moves;

        protected override void CalculateUnderAttackSquaresInternal()
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

        protected override bool CanEatAtInternal(Square square)
        {
            return CanMoveToInternal(square);
        }

        protected override bool CanMoveToInternal(Square square)
        {
            if (square.HasPiece() && square.GetPieceColor() == pieceColor)
            {
                return false;
            }

            CalculateUnderAttackSquares();

            foreach (var underAttackSquare in UnderAttackSquares)
            {
                if (square == underAttackSquare)
                {
                    return true;
                }
            }

            return false;
        }
    }
}