using System.Collections.Generic;
using System.Linq;
using Logic;
using Logic.Notation;
using UnityEngine;

namespace Board.Pieces
{
    public class King : Piece
    {
        [Header("King")]
        public Vector2Int[] Moves;
        [SerializeField] private List<Square> castlingSquares;

        public List<Square> CastlingSquares => castlingSquares;

        public override void CalculateMovesAndCaptures()
        {
            MoveSquares.Clear();
            CaptureSquares.Clear();
            CannotMoveSquares.Clear();
            CastlingSquares.Clear();

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
                    if (gameManager.UnderAttackSquares.Contains(square))
                    {
                        CannotMoveSquares.Add(square);
                    }
                    else
                    {
                        MoveSquares.Add(square);
                    }
                }
                else if (square.GetPieceColor() != pieceColor)
                {
                    CaptureSquares.Add(square);
                }
            }

            // Add castling
            Square shortCastlingSquare = gameManager.GetSquare(pieceColor, currentSquare, new Vector2Int(2, 0));
            Square longCastlingSquare = gameManager.GetSquare(pieceColor, currentSquare, new Vector2Int(-2, 0));

            List<Square> possibleSquares = new();
            if (CanCastlingAt(shortCastlingSquare, out _, out _, out _))
            {
                possibleSquares.Add(shortCastlingSquare);
            }

            if (CanCastlingAt(longCastlingSquare, out _, out _, out _))
            {
                possibleSquares.Add(longCastlingSquare);
            }

            // Can not castle under attack
            if (gameManager.CheckType == CheckType.None)
            {
                CastlingSquares.AddRange(possibleSquares);
            }
            else
            {
                CannotMoveSquares.AddRange(possibleSquares);
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

        public bool CanCastlingAt(Square square, out Rook rook, out Square rookSquare, out NotationTurnType notationTurnType)
        {
            rook = null;
            rookSquare = null;
            notationTurnType = NotationTurnType.None;

            if (!IsFirstMove)
            {
                return false;
            }

            // Check short (right)
            Square squarePlus1 = gameManager.GetSquare(pieceColor, currentSquare, new Vector2Int(1, 0));
            Square squarePlus2 = gameManager.GetSquare(pieceColor, currentSquare, new Vector2Int(2, 0));
            Square squareWithRook = gameManager.GetSquare(pieceColor, currentSquare, new Vector2Int(3, 0));

            if (CanCastleRight(out rook))
            {
                rookSquare = squarePlus1;
                notationTurnType = NotationTurnType.CastlingShort;

                return true;
            }

            // Check long (left)
            Square squareMinus1 = gameManager.GetSquare(pieceColor, currentSquare, new Vector2Int(-1, 0));
            Square squareMinus2 = gameManager.GetSquare(pieceColor, currentSquare, new Vector2Int(-2, 0));
            Square squareMinus3 = gameManager.GetSquare(pieceColor, currentSquare, new Vector2Int(-3, 0));
            squareWithRook = gameManager.GetSquare(pieceColor, currentSquare, new Vector2Int(-4, 0));

            if (CanCastleLeft(out rook))
            {
                rookSquare = squareMinus1;
                notationTurnType = NotationTurnType.CastlingLong;

                return true;
            }

            return false;

            bool CanCastleRight(out Rook rook)
            {
                rook = null;
                bool isSquares = !squarePlus1.HasPiece() && !squarePlus2.HasPiece() && squarePlus2.IsEqual(square);
                bool isRook = squareWithRook.HasPiece() && squareWithRook.GetPiece() is Rook r && r.IsFirstMove;

                if (isRook)
                {
                    rook = squareWithRook.GetPiece() as Rook;
                }

                bool isNotUnderAttack = !gameManager.UnderAttackSquares.Contains(squarePlus1)
                                        && !gameManager.UnderAttackSquares.Contains(squarePlus2);

                return isSquares && isRook && isNotUnderAttack;
            }

            bool CanCastleLeft(out Rook rook)
            {
                rook = null;
                bool isSquares = !squareMinus1.HasPiece() && !squareMinus2.HasPiece()
                                                          && !squareMinus3.HasPiece() && squareMinus2.IsEqual(square);
                bool isRook = squareWithRook.HasPiece() && squareWithRook.GetPiece() is Rook r && r.IsFirstMove;

                if (isRook)
                {
                    rook = squareWithRook.GetPiece() as Rook;
                }

                bool isNotUnderAttack = !gameManager.UnderAttackSquares.Contains(squareMinus1)
                                        && !gameManager.UnderAttackSquares.Contains(squareMinus2);

                return isSquares && isRook && isNotUnderAttack;
            }
        }

        protected override bool CanEatAtInternal(Square square)
        {
            return CaptureSquares.Contains(square);
        }

        protected override bool CanMoveToInternal(Square square)
        {
            return MoveSquares.Contains(square);
        }
    }
}
