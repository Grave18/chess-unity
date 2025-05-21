using ChessGame.ChessBoard;
using ChessGame.ChessBoard.Pieces;

namespace ChessGame.Logic.Moves
{
    internal class MovePromotion : Turn
    {
        private readonly SimpleMove _simpleMove;
        private readonly Promotion _promotion;

        public MovePromotion(Piece piece, Square fromSquare, Square toSquare, Piece otherPiece)
        {
            _simpleMove = new SimpleMove(piece, fromSquare, toSquare, isFirstMove: false);
            _promotion = new Promotion(piece, fromSquare, toSquare, otherPiece);
        }

        public override void Progress(float t, bool isUndo = false)
        {
            _simpleMove.Progress(t);
        }

        public override void End()
        {
            _simpleMove.End();
            _promotion.End();
        }

        public override void EndUndo()
        {
            _simpleMove.EndUndo();
            _promotion.EndUndo();
        }

        public override void PlaySound()
        {
            base.PlaySound();
            EffectsPlayer.Instance.PlayPromoteSound();
        }
    }
}