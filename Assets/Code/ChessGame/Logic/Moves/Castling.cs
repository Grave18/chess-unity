using ChessGame.ChessBoard.Info;

namespace ChessGame.Logic.Moves
{
    public class Castling : Turn
    {
        private readonly SimpleMove _kingMove;
        private readonly SimpleMove _rookMove;

        public Castling(CastlingInfo castlingInfo, bool isFirstMove)
        {
            _kingMove = new SimpleMove(castlingInfo.King, castlingInfo.KingFromSquare, castlingInfo.KingToSquare, isFirstMove);
            _rookMove = new SimpleMove(castlingInfo.Rook, castlingInfo.RookFromSquare, castlingInfo.RookToSquare, isFirstMove);
        }

        public override void Progress(float t)
        {
            if (t < 0.5f)
            {
                _kingMove.Progress(2*t);
            }
            else
            {
                _rookMove.Progress(2*t - 1);
            }
        }

        public override void End()
        {
            _kingMove.End();
            _rookMove.End();
        }

        public override void EndUndo()
        {
            _kingMove.EndUndo();
            _rookMove.EndUndo();
        }
    }
}