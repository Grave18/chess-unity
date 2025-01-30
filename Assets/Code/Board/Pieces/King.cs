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

        protected override void CalculateMovesAndCapturesInternal()
        {
            MoveSquares.Clear();
            CaptureSquares.Clear();
            CannotMoveSquares.Clear();
            CastlingSquares.Clear();

            // Calculate Moves and Captures
            foreach (Vector2Int offset in Moves)
            {
                Square square = gameManager.GetSquareRel(pieceColor, currentSquare, offset);

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

            // Add castling. Do need absolute position
            Square shortCastlingSquare = gameManager.GetSquareRel(PieceColor.White, currentSquare, new Vector2Int(2, 0));
            Square longCastlingSquare = gameManager.GetSquareRel(PieceColor.White, currentSquare, new Vector2Int(-2, 0));

            if (CanCastlingShort(shortCastlingSquare, out Rook shortRook, out _))
            {
                CastlingSquares.Add(shortCastlingSquare);
            }
            else if(shortRook != null && shortRook.IsFirstMove)
            {
                CannotMoveSquares.Add(shortCastlingSquare);
            }

            if (CanCastleLong(longCastlingSquare, out Rook longRook, out _))
            {
                CastlingSquares.Add(longCastlingSquare);
            }
            else if(longRook != null && longRook.IsFirstMove)
            {
                CannotMoveSquares.Add(longCastlingSquare);
            }
        }

        public bool CanCastlingAt(Square square, out Rook rook, out Square rookSquare, out NotationTurnType notationTurnType)
        {
            rook = null;
            rookSquare = null;
            notationTurnType = NotationTurnType.None;

            if (CanCastlingShort(square, out Rook sRook, out Square sRookSquare))
            {
                rook = sRook;
                rookSquare = sRookSquare;
                notationTurnType = NotationTurnType.CastlingShort;
                return true;
            }

            if (CanCastleLong(square, out Rook lRook, out Square lRookSquare))
            {
                rook = lRook;
                rookSquare = lRookSquare;
                notationTurnType = NotationTurnType.CastlingLong;
                return true;
            }

            return false;
        }

        private bool CanCastlingShort(Square square, out Rook rook, out Square rookSquare)
        {
            rook = null;
            rookSquare = null;

            if (!IsFirstMove)
            {
                return false;
            }

            // Piece color must be White because black king is not mirrored. Don't need relative position
            // Check short
            Square squarePlus1 = gameManager.GetSquareRel(PieceColor.White, currentSquare, new Vector2Int(1, 0));
            Square squarePlus2 = gameManager.GetSquareRel(PieceColor.White, currentSquare, new Vector2Int(2, 0));
            Square squareWithShortRook = gameManager.GetSquareRel(PieceColor.White, currentSquare, new Vector2Int(3, 0));

            bool isSquares = !squarePlus1.HasPiece() && !squarePlus2.HasPiece() && squarePlus2.IsEqual(square);
            bool isRook = squareWithShortRook.HasPiece() && squareWithShortRook.GetPiece() is Rook r && r.IsFirstMove;

            if (isRook)
            {
                rook = squareWithShortRook.GetPiece() as Rook;
            }

            bool isNotUnderAttack = gameManager.CheckType == CheckType.None
                                    && !gameManager.UnderAttackSquares.Contains(squarePlus1)
                                    && !gameManager.UnderAttackSquares.Contains(squarePlus2);

            rookSquare = squarePlus1;
            return isSquares && isRook && isNotUnderAttack;
        }

        private bool CanCastleLong(Square square, out Rook rook, out Square rookSquare)
        {
            rook = null;
            rookSquare = null;

            // Check long
            Square squareMinus1 = gameManager.GetSquareRel(PieceColor.White, currentSquare, new Vector2Int(-1, 0));
            Square squareMinus2 = gameManager.GetSquareRel(PieceColor.White, currentSquare, new Vector2Int(-2, 0));
            Square squareMinus3 = gameManager.GetSquareRel(PieceColor.White, currentSquare, new Vector2Int(-3, 0));
            Square squareWithLongRook = gameManager.GetSquareRel(PieceColor.White, currentSquare, new Vector2Int(-4, 0));

            bool isSquares = !squareMinus1.HasPiece() && !squareMinus2.HasPiece()
                                                      && !squareMinus3.HasPiece() && squareMinus2.IsEqual(square);
            bool isRook = squareWithLongRook.HasPiece() && squareWithLongRook.GetPiece() is Rook r && r.IsFirstMove;

            if (isRook)
            {
                rook = squareWithLongRook.GetPiece() as Rook;
            }

            bool isNotUnderAttack = gameManager.CheckType == CheckType.None
                                    && !gameManager.UnderAttackSquares.Contains(squareMinus1)
                                    && !gameManager.UnderAttackSquares.Contains(squareMinus2);

            rookSquare = squareMinus1;
            return isSquares && isRook && isNotUnderAttack;
        }
    }
}
