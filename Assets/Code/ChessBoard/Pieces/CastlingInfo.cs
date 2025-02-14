using Logic.Notation;

namespace ChessBoard.Pieces
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