using System.Collections.Generic;
using ChessGame.ChessBoard.Info;
using ChessGame.Logic;
using ChessGame.Logic.MovesBuffer;
using UnityEngine;

namespace ChessGame.ChessBoard.Pieces
{
    public class King : Piece
    {
        [Header("King")]
        [SerializeField] private Vector2Int[] moves;

        public List<CastlingInfo> CastlingSquares { get; } = new();

        public override void CalculateConstrains()
        {
            var moveSquaresTemp = new List<Square>(MoveSquares.Keys);
            foreach (Square square in moveSquaresTemp)
            {
                if (Game.UnderAttackSquares.Contains(square)
                    || Game.AttackLines.ContainsBehindKingSquare(square))
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
                Square square = Game.GetSquareRel(pieceColor, currentSquare, offset);

                if (square == Game.NullSquare)
                {
                    continue;
                }

                if (!square.HasPiece())
                {
                    MoveSquares.Add(square, new MoveInfo());
                }
                // Defend square where same color piece stands
                else if (square.GetPieceColor() == pieceColor)
                {
                    DefendSquares.Add(square);
                }
                // Capture
                else if (square.GetPieceColor() != pieceColor
                         && !Game.UnderAttackSquares.Contains(square))
                {
                    CaptureSquares.Add(square, new CaptureInfo(square.GetPiece()));
                }
            }

            // Add short castling. Do need absolute position
            Square shortCastlingSquare = Game.GetSquareAbs(currentSquare, new Vector2Int(2, 0));
            if (CanCastlingShort(shortCastlingSquare, out CastlingInfo shortCastlingInfo))
            {
                CastlingSquares.Add(shortCastlingInfo);
            }
            else if(shortCastlingInfo.IsBlocked)
            {
                CannotMoveSquares.Add(shortCastlingSquare);
            }

            // Add long castling. Do need absolute position
            Square longCastlingSquare = Game.GetSquareAbs(currentSquare, new Vector2Int(-2, 0));
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
                if (ci.KingToSquare.IsEqual(square))
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
            Square squarePlus1 = Game.GetSquareAbs(currentSquare, new Vector2Int(1, 0));
            Square squarePlus2 = Game.GetSquareAbs(currentSquare, new Vector2Int(2, 0));
            Square squareWithShortRook = Game.GetSquareAbs(currentSquare, new Vector2Int(3, 0));

            // Conditions
            bool isSquaresHasNoPiece = !squarePlus1.HasPiece() && !squarePlus2.HasPiece() && squarePlus2.IsEqual(square);
            bool isSquaresUnderAttack = Game.UnderAttackSquares.Contains(squarePlus1)
                                    || Game.UnderAttackSquares.Contains(squarePlus2);
            bool isRook = squareWithShortRook.HasPiece() && squareWithShortRook.GetPiece() is Rook { IsFirstMove: true };

            // Set castling info
            castlingInfo.King = this;
            castlingInfo.Rook = isRook ? squareWithShortRook.GetPiece() as Rook : null;
            castlingInfo.KingFromSquare = GetSquare();
            castlingInfo.KingToSquare = square;
            castlingInfo.RookFromSquare = castlingInfo.Rook?.GetSquare();
            castlingInfo.RookToSquare = squarePlus1;
            castlingInfo.MoveType = MoveType.CastlingShort;
            castlingInfo.IsBlocked = castlingInfo.Rook != null
                                     && IsFirstMove
                                     && (isSquaresUnderAttack || Game.CheckType is CheckType.Check or CheckType.DoubleCheck);

            return IsFirstMove && isSquaresHasNoPiece && isRook && !isSquaresUnderAttack && Game.CheckType == CheckType.None;
        }

        private bool CanCastleLong(Square square, out CastlingInfo castlingInfo)
        {
            // Squares to the left
            Square squareMinus1 = Game.GetSquareAbs(currentSquare, new Vector2Int(-1, 0));
            Square squareMinus2 = Game.GetSquareAbs(currentSquare, new Vector2Int(-2, 0));
            Square squareMinus3 = Game.GetSquareAbs(currentSquare, new Vector2Int(-3, 0));
            Square squareWithLongRook = Game.GetSquareAbs(currentSquare, new Vector2Int(-4, 0));

            // Conditions
            bool isSquaresHasNoPiece = !squareMinus1.HasPiece() && !squareMinus2.HasPiece()
                                                      && !squareMinus3.HasPiece() && squareMinus2.IsEqual(square);
            bool isSquaresUnderAttack = Game.UnderAttackSquares.Contains(squareMinus1)
                                        || Game.UnderAttackSquares.Contains(squareMinus2);
            bool isRook = squareWithLongRook.HasPiece() && squareWithLongRook.GetPiece() is Rook { IsFirstMove: true };

            // Set castling info
            castlingInfo.King = this;
            castlingInfo.Rook = isRook ? squareWithLongRook.GetPiece() as Rook : null;
            castlingInfo.KingFromSquare = GetSquare();
            castlingInfo.KingToSquare = square;
            castlingInfo.RookFromSquare = castlingInfo.Rook?.GetSquare();
            castlingInfo.RookToSquare = squareMinus1;
            castlingInfo.MoveType = MoveType.CastlingLong;
            castlingInfo.IsBlocked = castlingInfo.Rook != null
                                     && IsFirstMove
                                     && (isSquaresUnderAttack || Game.CheckType is CheckType.Check or CheckType.DoubleCheck);

            return IsFirstMove && isSquaresHasNoPiece && isRook && !isSquaresUnderAttack && Game.CheckType == CheckType.None;
        }
    }
}
