namespace ChessGame.ChessBoard.Info
{
    public class MoveInfo
    {
        public Square EnPassantSquare { get; }

        public MoveInfo(Square enPassantSquare = null)
        {
            EnPassantSquare = enPassantSquare;
        }
    }
}