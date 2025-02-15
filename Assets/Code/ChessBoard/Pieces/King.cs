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
            var moveSquaresTemp = new List<Square>(MoveSquares);
            foreach (Square square in moveSquaresTemp)
            {
                if (game.UnderAttackSquares.Contains(square))
                {
                    MoveSquares.Remove(square);
                    CannotMoveSquares.Add(square);
                }
            }
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
                    MoveSquares.Add(square);
                }
                // Defend square where same color piece stands
                else if (square.GetPieceColor() == pieceColor)
                {
                    DefendSquares.Add(square);
                }
                // Capture
                else if (square.GetPieceColor() != pieceColor
                         && !game.UnderAttackSquares.Contains(square))
                {
                    CaptureSquares.Add(square, new CaptureInfo(square.GetPiece()));
                }
            }

            // Add short castling. Do need absolute position
            Square shortCastlingSquare = game.GetSquareAbs(currentSquare, new Vector2Int(2, 0));
            if (CanCastlingShort(shortCastlingSquare, out CastlingInfo shortCastlingInfo))
            {
                CastlingSquares.Add(shortCastlingInfo);
            }
            else if(shortCastlingInfo.IsBlocked)
            {
                CannotMoveSquares.Add(shortCastlingSquare);
            }

            // Add long castling. Do need absolute position
            Square longCastlingSquare = game.GetSquareAbs(currentSquare, new Vector2Int(-2, 0));
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
            // Squares to the right
            Square squarePlus1 = game.GetSquareAbs(currentSquare, new Vector2Int(1, 0));
            Square squarePlus2 = game.GetSquareAbs(currentSquare, new Vector2Int(2, 0));
            Square squareWithShortRook = game.GetSquareAbs(currentSquare, new Vector2Int(3, 0));

            // Conditions
            bool isSquaresHasNoPiece = !squarePlus1.HasPiece() && !squarePlus2.HasPiece() && squarePlus2.IsEqual(square);
            bool isSquaresUnderAttack = game.UnderAttackSquares.Contains(squarePlus1)
                                    || game.UnderAttackSquares.Contains(squarePlus2);
            bool isRook = squareWithShortRook.HasPiece() && squareWithShortRook.GetPiece() is Rook { IsFirstMove: true };

            // Set castling info
            castlingInfo.Rook = isRook ? squareWithShortRook.GetPiece() as Rook : null;
            castlingInfo.CastlingSquare = square;
            castlingInfo.RookSquare = squarePlus1;
            castlingInfo.NotationTurnType = NotationTurnType.CastlingShort;
            castlingInfo.IsBlocked = castlingInfo.Rook != null
                                     && IsFirstMove
                                     && (isSquaresUnderAttack || game.CheckType is CheckType.Check or CheckType.DoubleCheck);

            return IsFirstMove && isSquaresHasNoPiece && isRook && !isSquaresUnderAttack && game.CheckType == CheckType.None;
        }

        private bool CanCastleLong(Square square, out CastlingInfo castlingInfo)
        {
            // Squares to the left
            Square squareMinus1 = game.GetSquareAbs(currentSquare, new Vector2Int(-1, 0));
            Square squareMinus2 = game.GetSquareAbs(currentSquare, new Vector2Int(-2, 0));
            Square squareMinus3 = game.GetSquareAbs(currentSquare, new Vector2Int(-3, 0));
            Square squareWithLongRook = game.GetSquareAbs(currentSquare, new Vector2Int(-4, 0));

            // Conditions
            bool isSquaresHasNoPiece = !squareMinus1.HasPiece() && !squareMinus2.HasPiece()
                                                      && !squareMinus3.HasPiece() && squareMinus2.IsEqual(square);
            bool isSquaresUnderAttack = game.UnderAttackSquares.Contains(squareMinus1)
                                        || game.UnderAttackSquares.Contains(squareMinus2);
            bool isRook = squareWithLongRook.HasPiece() && squareWithLongRook.GetPiece() is Rook { IsFirstMove: true };

            // Set castling info
            castlingInfo.Rook = isRook ? squareWithLongRook.GetPiece() as Rook : null;
            castlingInfo.CastlingSquare = square;
            castlingInfo.RookSquare = squareMinus1;
            castlingInfo.NotationTurnType = NotationTurnType.CastlingLong;
            castlingInfo.IsBlocked = castlingInfo.Rook != null
                                     && IsFirstMove
                                     && (isSquaresUnderAttack || game.CheckType is CheckType.Check or CheckType.DoubleCheck);

            return IsFirstMove && isSquaresHasNoPiece && isRook && !isSquaresUnderAttack && game.CheckType == CheckType.None;
        }
    }
}
