using ChessBoard.Info;
using ChessBoard.Pieces;

namespace Logic.MovesBuffer
{
    public class MoveData
    {
        public string Uci;
        public MoveType MoveType;
        public bool IsFirstMove;
        public Piece BeatenPiece;
        public CastlingInfo CastlingInfo;
        public string EpSquareAddress = "-";
    }
}