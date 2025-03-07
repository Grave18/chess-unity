using ChessBoard;
using ChessBoard.Pieces;

namespace Logic.CommandPattern
{
    public class EnPassantInfo
    {
        public Square Square { get; set; }
        public Piece Piece { get; set; }
    }
}