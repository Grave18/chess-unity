using UnityEngine;

namespace Board.Pieces
{
    public class Rook : Piece
    {
        [Header("Rook")]
        public Vector2Int[] MoveVectors;

        public override void CalculateMovesAndCaptures()
        {
            MoveSquares.Clear();
            CaptureSquares.Clear();

            foreach (Vector2Int direction in MoveVectors)
            {
                var offset = direction;
                while (true)
                {
                    var square = gameManager.GetSquare(pieceColor, currentSquare, offset);
                    offset += direction;

                    if (square == gameManager.NullSquare)
                    {
                        break;
                    }

                    if (square.HasPiece())
                    {
                        if (square.GetPieceColor() != pieceColor)
                        {
                            CaptureSquares.Add(square);
                        }

                        break;
                    }

                    MoveSquares.Add(square);
                }
            }
        }

        protected override void CalculateUnderAttackSquaresInternal()
        {
            UnderAttackSquares.Clear();

            foreach (Vector2Int direction in MoveVectors)
            {
                var currentDir = direction;

                while (true)
                {
                    var underAttackSquare = gameManager.GetSquare(pieceColor, currentSquare, currentDir);
                    // Advance for one square
                    currentDir += direction;

                    // Has Piece
                    if (underAttackSquare.HasPiece())
                    {
                        if (underAttackSquare.GetPieceColor() == pieceColor)
                        {
                            break;
                        }

                        UnderAttackSquares.Add(underAttackSquare);
                        break;
                    }

                    // Off board
                    if (underAttackSquare == gameManager.NullSquare)
                    {
                        break;
                    }

                    // Empty Square
                    UnderAttackSquares.Add(underAttackSquare);
                }
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