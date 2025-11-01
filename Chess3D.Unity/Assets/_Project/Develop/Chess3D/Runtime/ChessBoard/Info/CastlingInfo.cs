using Chess3D.Runtime.ChessBoard.Pieces;
using Chess3D.Runtime.Logic.MovesBuffer;

namespace Chess3D.Runtime.ChessBoard.Info
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