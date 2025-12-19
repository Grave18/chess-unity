using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.ChessBoard.Info;

namespace Chess3D.Runtime.Core.AI
{
    public sealed class AICalculationsResult
    {
        public Square MoveFrom { get; }
        public Square MoveTo { get; }
        public PieceType Promotion { get; }

        public AICalculationsResult(Square moveFrom, Square moveTo, PieceType promotion)
        {
            MoveFrom = moveFrom;
            MoveTo = moveTo;
            Promotion = promotion;
        }
    }
}