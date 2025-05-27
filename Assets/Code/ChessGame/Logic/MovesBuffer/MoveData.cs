using ChessGame.ChessBoard.Info;
using ChessGame.ChessBoard.Pieces;

namespace ChessGame.Logic.MovesBuffer
{
    public class MoveData
    {
        public string Uci;
        public MoveType MoveType;
        public bool IsFirstMove;
        public string ThreefoldShortFen;
        public int Rule50Count;
        public string EpSquareAddress = "-";
        public string AlgebraicNotation;
        public PieceColor TurnColor;

        public Piece HiddenPawn;
        public Piece BeatenPiece;
        public CastlingInfo CastlingInfo;
    }
}