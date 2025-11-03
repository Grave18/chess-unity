using Chess3D.Runtime.Core.ChessBoard.Info;
using Chess3D.Runtime.Core.ChessBoard.Pieces;
using Chess3D.Runtime.Core.Logic.GameStates;

namespace Chess3D.Runtime.Core.Logic.MovesBuffer
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