using ChessBoard.Info;
using ChessBoard.Pieces;
using Logic.GameStates;

namespace Logic.MovesBuffer
{
    public class MoveData
    {
        public string Uci;
        public MoveType MoveType;
        public bool IsFirstMove;
        public string ThreefoldShortFen;
        public int HalfMoveClock;
        public int FullMoveCounter = 1;
        public string EpSquareAddress = "-";
        public string AlgebraicNotation;
        public PieceColor TurnColor;

        public Piece HiddenPawn;
        public Piece BeatenPiece;
        public CastlingInfo CastlingInfo;

        public IGameState PreviousState;
    }
}