using System.Linq;
using Logic;
using UnityEngine;

namespace Board.Pieces
{
    public class LongRange : Piece
    {
        [Header("Long Range")]
        [SerializeField] private Vector2Int[] moveVectors;
        public Vector2Int[] MoveVectors => moveVectors;

        protected override void CalculateMovesAndCapturesInternal()
        {
            MoveSquares.Clear();
            CaptureSquares.Clear();

            foreach (Vector2Int direction in MoveVectors)
            {
                Vector2Int offset = direction;
                while (true)
                {
                    Square square = gameManager.GetSquareRel(pieceColor, currentSquare, offset);
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
    }
}