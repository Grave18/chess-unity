using System.Text;
using ChessBoard;
using ChessBoard.Pieces;
using Logic;
using Logic.MovesBuffer;

namespace Notation
{
    public static class SeriesList
    {
        /// Get algebraic turn notation. Example: "e2e4", "0-0", ...
        public static string Get(Piece piece, Square fromSquare, Square toSquare, MoveType moveType,
            CheckType checkType, Piece promotedPiece)
        {
            // Castling
            if(moveType == MoveType.CastlingShort)
            {
                return "0-0";
            }

            if (moveType == MoveType.CastlingLong)
            {
                return "0-0-0";
            }

            var turnSb = new StringBuilder();
            // Piece name
            bool isCaptureOrEnPassant = moveType is MoveType.Capture or MoveType.EnPassant;
            switch (piece)
            {
                case King:   turnSb.Append("K"); break;
                case Queen:  turnSb.Append("Q"); break;
                case Rook:   turnSb.Append("R"); break;
                case Bishop: turnSb.Append("B"); break;
                case Knight: turnSb.Append("N"); break;
                case Pawn:
                    if (isCaptureOrEnPassant)
                    {
                        turnSb.Append(fromSquare.File);
                    }
                    break;
            }

            // Is capturing
            if (isCaptureOrEnPassant)
            {
                turnSb.Append("x");
            }

            // Square name
            turnSb.Append(toSquare.Address);

            // Is En Passant
            if (moveType == MoveType.EnPassant)
            {
                turnSb.Append(" e.p.");
            }
            // Is promotion
            else if (promotedPiece != null)
            {
                switch (promotedPiece)
                {
                    case Queen:  turnSb.Append("=Q"); break;
                    case Rook:   turnSb.Append("=R"); break;
                    case Bishop: turnSb.Append("=B"); break;
                    case Knight: turnSb.Append("=N"); break;
                }
            }

            // Is check
            switch (checkType)
            {
                case CheckType.Check:       turnSb.Append("+");  break;
                case CheckType.DoubleCheck: turnSb.Append("++"); break;
                case CheckType.CheckMate:   turnSb.Append("#");  break;
            }

            return turnSb.ToString();
        }
    }
}