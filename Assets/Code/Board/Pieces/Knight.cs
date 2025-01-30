using UnityEngine;

namespace Board.Pieces
{
    public class Knight : Piece
    {
        [Header("Knight")]
        public Vector2Int[] Moves;

        public override void CalculateMovesAndCaptures()
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

        protected override void CalculateUnderAttackSquaresInternal()
        {
            UnderAttackSquares.Clear();
            foreach (Vector2Int offset in Moves)
            {
                var underAttackSquare = gameManager.GetSquareRel(pieceColor, currentSquare, offset);

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