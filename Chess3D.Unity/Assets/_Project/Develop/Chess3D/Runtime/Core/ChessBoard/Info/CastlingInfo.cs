using Chess3D.Runtime.Core.ChessBoard.Pieces;
using Chess3D.Runtime.Core.Logic.MovesBuffer;

namespace Chess3D.Runtime.Core.ChessBoard.Info
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