using System.Collections.Generic;
using UnityEngine;

namespace ChessBoard.Pieces
{
    public class LongRange : Piece
    {
        [Header("Long Range")]
        [SerializeField] private Vector2Int[] moveVectors;
        private HashSet<Square> attackLineSquares = new();

        public HashSet<Square> AttackLineSquares => attackLineSquares;
        public bool HasAttackLine => attackLineSquares.Count > 0;

        protected override void CalculateMovesAndCapturesInternal()
        {
            foreach (Vector2Int direction in moveVectors)
            {
                Vector2Int offset = direction;
                HashSet<Square> possibleAttackLine = new();
                bool isFindingKing = false;
                while (true)
                {
                    Square square = game.GetSquareRel(pieceColor, currentSquare, offset);
                    offset += direction;

                    if (square == game.NullSquare)
                    {
                        break;
                    }

                    if (isFindingKing)
                    {
                        if (square.HasPiece())
                        {
                            if (square.GetPiece() is King king && king.GetPieceColor() != pieceColor)
                            {
                                attackLineSquares = possibleAttackLine;
                            }

                            break;
                        }

                        possibleAttackLine.Add(square);
                        continue;
                    }

                    if (square.HasPiece())
                    {
                        if (square.GetPieceColor() != pieceColor)
                        {
                            possibleAttackLine.Add(square);
                            CaptureSquares.Add(square, square.GetPiece());

                            // Find attack line to king
                            if (square.GetPiece() is King)
                            {
                                attackLineSquares = possibleAttackLine;
                                break;
                            }

                            isFindingKing = true;
                        }
                        else
                        {
                            // If same color
                            DefendSquares.Add(square);
                            break;
                        }
                    }
                    else
                    {
                        possibleAttackLine.Add(square);
                        MoveSquares.Add(square);
                    }
                }
            }
        }
    }
}