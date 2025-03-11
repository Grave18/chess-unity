using ChessBoard.Pieces;

namespace ChessBoard.Info
{
    public class EnPassantInfo
    {
        public Square Square { get;}
        public Piece Piece { get; }

        public EnPassantInfo(Piece piece, Square square)
        {
            Piece = piece;
            Square = square;
        }
    }
}