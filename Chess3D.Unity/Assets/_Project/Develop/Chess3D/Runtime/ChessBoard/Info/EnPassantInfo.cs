using Chess3D.Runtime.ChessBoard.Pieces;

namespace Chess3D.Runtime.ChessBoard.Info
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