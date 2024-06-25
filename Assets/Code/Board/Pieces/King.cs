﻿using Logic;
using UnityEngine;

namespace Board.Pieces
{
    public class King : Piece
    {
        [Header("King")]
        public Vector2Int[] Moves;

        public override void CalculateMovesAndCaptures()
        {
            MoveSquares.Clear();
            CaptureSquares.Clear();

            // Calculate Moves and Captures
            foreach (Vector2Int offset in Moves)
            {
                var square = gameManager.GetSquare(pieceColor, currentSquare, offset);

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

            // Add castling
            var shortCastlingSquare = gameManager.GetSquare(pieceColor, currentSquare, new Vector2Int(2, 0));
            var longCastlingSquare = gameManager.GetSquare(pieceColor, currentSquare, new Vector2Int(-2, 0));
            if (CanCastlingAt(shortCastlingSquare, out _, out _, out _))
            {
                MoveSquares.Add(shortCastlingSquare);
            }

            if (CanCastlingAt(longCastlingSquare, out _, out _, out _))
            {
                MoveSquares.Add(longCastlingSquare);
            }
        }

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

        public bool CanCastlingAt(Square square, out Rook rook, out Square rookSquare, out TurnType turnType)
        {
            rook = null;
            rookSquare = null;
            turnType = TurnType.None;

            if (!IsFirstMove)
            {
                return false;
            }

            // Check short (right)
            Square squarePlus1 = gameManager.GetSquare(pieceColor, currentSquare, new Vector2Int(1, 0));
            Square squarePlus2 = gameManager.GetSquare(pieceColor, currentSquare, new Vector2Int(2, 0));
            Square squareWithRook = gameManager.GetSquare(pieceColor, currentSquare, new Vector2Int(3, 0));

            if (!squarePlus1.HasPiece() && !squarePlus2.HasPiece() && squarePlus2.IsEqual(square) &&
                squareWithRook.HasPiece() &&
                squareWithRook.GetPiece() is Rook rookRight && rookRight.IsFirstMove)
            {
                rook = rookRight;
                rookSquare = squarePlus1;
                turnType = TurnType.CastlingShort;

                return true;
            }

            // Check long (left)
            Square squareMinus1 = gameManager.GetSquare(pieceColor, currentSquare, new Vector2Int(-1, 0));
            Square squareMinus2 = gameManager.GetSquare(pieceColor, currentSquare, new Vector2Int(-2, 0));
            Square squareMinus3 = gameManager.GetSquare(pieceColor, currentSquare, new Vector2Int(-3, 0));
            squareWithRook = gameManager.GetSquare(pieceColor, currentSquare, new Vector2Int(-4, 0));

            if (!squareMinus1.HasPiece() && !squareMinus2.HasPiece() && !squareMinus3.HasPiece() &&
                squareWithRook.HasPiece() &&
                squareMinus2.IsEqual(square) && squareWithRook.GetPiece() is Rook rookLeft && rookLeft.IsFirstMove)
            {
                rook = rookLeft;
                rookSquare = squareMinus1;
                turnType = TurnType.CastlingLong;

                return true;
            }

            return false;
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
                if (underAttackSquare == square)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
