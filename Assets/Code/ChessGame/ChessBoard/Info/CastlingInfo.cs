using ChessGame.ChessBoard.Pieces;
using ChessGame.Logic.MovesBuffer;

namespace ChessGame.ChessBoard.Info
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