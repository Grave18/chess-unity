using Chess3D.Runtime.ChessBoard;
using Chess3D.Runtime.ChessBoard.Info;

namespace Chess3D.Runtime.Logic
{
    public struct ParsedUci
    {
        public Square FromSquare;
        public Square ToSquare;
        public PieceType PromotedPieceType;
    }
}