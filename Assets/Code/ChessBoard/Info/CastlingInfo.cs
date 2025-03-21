using ChessBoard.Pieces;
using Logic.MovesBuffer;

namespace ChessBoard.Info
{
    public struct CastlingInfo
    {
        public Piece King;
        public Square KingFromSquare;
        public Square KingToSquare;
        public Rook Rook;
        public Square RookFromSquare;
        public Square RookToSquare;
        public bool IsBlocked;
        public MoveType MoveType;
    }
}