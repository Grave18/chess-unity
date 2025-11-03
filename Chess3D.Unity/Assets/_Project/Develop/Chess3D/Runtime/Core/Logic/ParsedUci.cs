using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.ChessBoard.Info;

namespace Chess3D.Runtime.Core.Logic
{
    public struct ParsedUci
    {
        public Square FromSquare;
        public Square ToSquare;
        public PieceType PromotedPieceType;
    }
}