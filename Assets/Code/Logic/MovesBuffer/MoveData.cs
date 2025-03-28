using ChessBoard.Info;
using ChessBoard.Pieces;

namespace Logic.MovesBuffer
{
    public class MoveData
    {
        public string Uci;
        public MoveType MoveType;
        public bool IsFirstMove;
        public string EpSquareAddress = "-";
        public string AlgebraicNotation;
        public PieceColor TurnColor;

        public Piece HiddenPawn;
        public Piece BeatenPiece;
        public CastlingInfo CastlingInfo;
    }
}