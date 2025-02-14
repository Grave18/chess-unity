using System.Collections.Generic;
using Logic;
using Logic.Notation;
using UnityEngine;
using UnityEngine.Serialization;

namespace ChessBoard.Pieces
{
    public class King : Piece
    {
        [Header("King")]
        [FormerlySerializedAs("Moves")]
        [SerializeField] private Vector2Int[] moves;

        public List<CastlingInfo> CastlingSquares { get; } = new();

        public override void CalculateConstrains()
        {
            // Constrains calculated in CalculateMovesAndCaptures
            // Common code don't needed
        }

        // Also calculate castling
        protected override void CalculateMovesAndCapturesInternal()
        {
            CastlingSquares.Clear();

            // Calculate Moves and Captures
            foreach (Vector2Int offset in moves)
            {
                Square square = game.GetSquareRel(pieceColor, currentSquare, offset);

                if (square == game.NullSquare)
                {
                    continue;
                }

                if (!square.HasPiece())
                {
                    if (game.UnderAttackSquares.Contains(square))
                    {
                        CannotMoveSquares.Add(square);
                    }
                    else
                    {
                        MoveSquares.Add(square);
                    }
                }
                else if (square.GetPieceColor() == pieceColor)
                {
                    DefendSquares.Add(square);
                }
                else if (square.GetPieceColor() != pieceColor
                         && !game.UnderAttackSquares.Contains(square))
                {
                    CaptureSquares.Add(square, square.GetPiece());
                }
            }

            // Add short castling. Do need absolute position
            Square shortCastlingSquare = game.GetSquareRel(PieceColor.White, currentSquare, new Vector2Int(2, 0));
            if (CanCastlingShort(shortCastlingSquare, out CastlingInfo shortCastlingInfo))
            {
                CastlingSquares.Add(shortCastlingInfo);
            }
            else if(shortCastlingInfo.IsBlocked)
            {
                CannotMoveSquares.Add(shortCastlingSquare);
            }

            // Add long castling. Do need absolute position
            Square longCastlingSquare = game.GetSquareRel(PieceColor.White, currentSquare, new Vector2Int(-2, 0));
            if (CanCastleLong(longCastlingSquare, out CastlingInfo longCastlingInfo))
            {
                CastlingSquares.Add(longCastlingInfo);
            }
            else if(longCastlingInfo.IsBlocked)
            {
                CannotMoveSquares.Add(longCastlingSquare);
            }
        }

        public bool CanCastlingAt(Square square, out CastlingInfo castlingInfo)
        {
            castlingInfo = default;

            foreach (CastlingInfo ci in CastlingSquares)
            {
                if (ci.CastlingSquare.IsEqual(square))
                {
                    castlingInfo = ci;
                    return true;
                }
            }

            return false;
        }

        private bool CanCastlingShort(Square square, out CastlingInfo castlingInfo)
        {
            castlingInfo = default;

            // Piece color must be White because black king is not mirrored. Don't need relative position
            // Check short
            Square squarePlus1 = game.GetSquareRel(PieceColor.White, currentSquare, new Vector2Int(1, 0));
            Square squarePlus2 = game.GetSquareRel(PieceColor.White, currentSquare, new Vector2Int(2, 0));
            Square squareWithShortRook = game.GetSquareRel(PieceColor.White, currentSquare, new Vector2Int(3, 0));

            bool isSquares = !squarePlus1.HasPiece() && !squarePlus2.HasPiece() && squarePlus2.IsEqual(square);
            bool isRook = squareWithShortRook.HasPiece() && squareWithShortRook.GetPiece() is Rook r && r.IsFirstMove;

            if (isRook)
            {
                castlingInfo.Rook = squareWithShortRook.GetPiece() as Rook;
            }

            bool isNotUnderAttack = !game.UnderAttackSquares.Contains(squarePlus1)
                                    && !game.UnderAttackSquares.Contains(squarePlus2);

            castlingInfo.CastlingSquare = square;
            castlingInfo.RookSquare = squarePlus1;
            castlingInfo.IsBlocked = !isNotUnderAttack || game.CheckType != CheckType.None;
            castlingInfo.NotationTurnType = NotationTurnType.CastlingShort;

            return IsFirstMove && isSquares && isRook && isNotUnderAttack && game.CheckType == CheckType.None;
        }

        private bool CanCastleLong(Square square, out CastlingInfo castlingInfo)
        {
            castlingInfo = default;

            // Check long
            Square squareMinus1 = game.GetSquareRel(PieceColor.White, currentSquare, new Vector2Int(-1, 0));
            Square squareMinus2 = game.GetSquareRel(PieceColor.White, currentSquare, new Vector2Int(-2, 0));
            Square squareMinus3 = game.GetSquareRel(PieceColor.White, currentSquare, new Vector2Int(-3, 0));
            Square squareWithLongRook = game.GetSquareRel(PieceColor.White, currentSquare, new Vector2Int(-4, 0));

            bool isSquares = !squareMinus1.HasPiece() && !squareMinus2.HasPiece()
                                                      && !squareMinus3.HasPiece() && squareMinus2.IsEqual(square);
            bool isRook = squareWithLongRook.HasPiece() && squareWithLongRook.GetPiece() is Rook r && r.IsFirstMove;

            if (isRook)
            {
                castlingInfo.Rook = squareWithLongRook.GetPiece() as Rook;
            }

            bool isNotUnderAttack = !game.UnderAttackSquares.Contains(squareMinus1)
                                    && !game.UnderAttackSquares.Contains(squareMinus2);

            castlingInfo.CastlingSquare = square;
            castlingInfo.RookSquare = squareMinus1;
            castlingInfo.IsBlocked = !isNotUnderAttack || game.CheckType != CheckType.None;
            castlingInfo.NotationTurnType = NotationTurnType.CastlingLong;

            return IsFirstMove && isSquares && isRook && isNotUnderAttack && game.CheckType == CheckType.None;
        }
    }
}
