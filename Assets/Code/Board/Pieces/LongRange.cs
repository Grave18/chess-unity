using System.Collections.Generic;
using System.Linq;
using Logic;
using UnityEngine;

namespace Board.Pieces
{
    public class LongRange : Piece
    {
        [Header("Long Range")]
        [SerializeField] private Vector2Int[] moveVectors;
        [SerializeField] private List<Square> attackLineSquares;

        public List<Square> AttackLineSquares => attackLineSquares;

        protected override void CalculateMovesAndCapturesInternal()
        {
            foreach (Vector2Int direction in moveVectors)
            {
                Vector2Int offset = direction;
                List<Square> possibleAttackLine = new();
                while (true)
                {
                    Square square = gameManager.GetSquareRel(pieceColor, currentSquare, offset);
                    offset += direction;

                    if (square == gameManager.NullSquare)
                    {
                        MoveSquares.AddRange(possibleAttackLine);
                        break;
                    }

                    if (square.HasPiece())
                    {
                        if (square.GetPieceColor() != pieceColor)
                        {
                            CaptureSquares.Add(square);

                            // Find attack line to king
                            if (square.GetPiece() is King)
                            {
                                attackLineSquares = possibleAttackLine;
                            }
                        }

                        MoveSquares.AddRange(possibleAttackLine);
                        break;
                    }

                    possibleAttackLine.Add(square);
                }
            }
        }
    }
}