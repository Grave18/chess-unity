using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.ChessBoard.Info;

namespace Chess3D.Runtime.Core.Ai
{
    public class AiCalculationsResult
    {
        public Square MoveFrom { get; }
        public Square MoveTo { get; }
        public PieceType Promotion { get; }

        public AiCalculationsResult(Square moveFrom, Square moveTo, PieceType promotion)
        {
            MoveFrom = moveFrom;
            MoveTo = moveTo;
            Promotion = promotion;
        }
    }
}