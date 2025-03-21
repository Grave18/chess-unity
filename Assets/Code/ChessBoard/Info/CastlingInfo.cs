using ChessBoard.Pieces;
using Logic.MovesBuffer;

namespace ChessBoard.Info
{
    public struct CastlingInfo
    {
        public Rook Rook;
        public Square CastlingSquare;
        public Square RookSquare;
        public bool IsBlocked;
        public MoveType MoveType;
    }
}