using System.Linq;
using Logic;
using UnityEngine;

namespace Board.Pieces
{
    public class Bishop : Piece
    {
        [Header("Bishop")]
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
                    var square = gameManager.GetSquareRel(pieceColor, currentSquare, offset);
                    offset += direction;

                    if (square == gameManager.NullSquare)
                    {
                        break;
                    }

                    if (gameManager.CheckType == CheckType.None)
                    {
                        if (square.HasPiece())
                        {
                            if (square.GetPieceColor() != pieceColor)
                            {
                                CaptureSquares.Add(square);
                            }
                            // If piece will be captured, then square can be under attack
                            else
                            {
                                MoveSquares.Add(square);
                            }

                            break;
                        }
                        else
                        {
                            MoveSquares.Add(square);
                        }
                    }
                    else if(gameManager.CheckType == CheckType.Check)
                    {
                        if (square.HasPiece())
                        {
                            // Only if captured piece is attacking king
                            if (square.GetPiece() == gameManager.Attackers.First())
                            {
                                CaptureSquares.Add(square);
                            }

                            break;
                        }
                        else
                        {
                            // Only if square lies on attack line
                            if (gameManager.AttackLine.Contains(square))
                            {
                                MoveSquares.Add(square);
                            }
                        }
                    }
                    // Can't do any if DoubleCheck
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
                    var underAttackSquare = gameManager.GetSquareRel(pieceColor, currentSquare, currentDir);
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