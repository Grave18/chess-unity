using AlgebraicNotation;
using ChessBoard.Pieces;

namespace ChessBoard.Info
{
    public struct CastlingInfo
    {
        public Rook Rook;
        public Square CastlingSquare;
        public Square RookSquare;
        public bool IsBlocked;
        public NotationTurnType NotationTurnType;
    }
}