namespace Chess3D.Runtime.Core.ChessBoard.Info
{
    public sealed class MoveInfo
    {
        public Square EnPassantSquare { get; }

        public MoveInfo(Square enPassantSquare = null)
        {
            EnPassantSquare = enPassantSquare;
        }
    }
}