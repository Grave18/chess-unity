using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.ChessBoard.Pieces;
using Chess3D.Runtime.Core.Sound;

namespace Chess3D.Runtime.Core.Logic.Moves
{
    internal class MovePromotion : Turn
    {
        private readonly SimpleMove _simpleMove;
        private readonly Promotion _promotion;

        public MovePromotion(Game game, Piece piece, Square fromSquare, Square toSquare, Piece otherPiece)
        {
            _simpleMove = new SimpleMove(game, piece, fromSquare, toSquare, isFirstMove: false);
            _promotion = new Promotion(piece, fromSquare, toSquare, otherPiece);
        }

        public override void Progress(float t)
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
            EffectsPlayer.Instance.PlayPromoteSound();
        }
    }
}