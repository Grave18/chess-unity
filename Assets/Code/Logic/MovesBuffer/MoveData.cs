using ChessBoard.Info;

namespace Logic.MovesBuffer
{
    public class MoveData
    {
        public string Uci;
        public MoveType MoveType;
        public bool IsFirstMove;
        public string EpSquareAddress = "-";
        public CastlingInfo CastlingInfo;
    }
}