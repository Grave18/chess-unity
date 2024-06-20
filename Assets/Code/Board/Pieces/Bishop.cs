﻿using UnityEngine;

namespace Board.Pieces
{
    public class Bishop : Piece
    {
        [Header("Bishop")]
        public Vector2Int[] MoveVectors;

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