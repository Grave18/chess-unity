using ChessBoard;
using ChessBoard.Pieces;
using Sound;

namespace Logic.Moves
{
    public class CapturePromotion : Turn
    {
        private readonly Capture _capture;
        private readonly Promotion _promotion;

        public CapturePromotion(Game game, Piece piece, Square fromSquare, Square toSquare, Piece otherPiece, Piece beatenPiece)
        {
            _capture = new Capture(game, piece, fromSquare, toSquare, beatenPiece, isFirstMove: false);
            _promotion = new Promotion(piece, fromSquare, toSquare, otherPiece);
        }

        public override void Progress(float t)
        {
            _capture.Progress(t);
        }

        public override void End()
        {
            _capture.End();
            _promotion.End();
        }

        public override void EndUndo()
        {
            _capture.EndUndo();
            _promotion.EndUndo();
        }

        public override void PlaySound()
        {
            EffectsPlayer.Instance.PlayPromoteSound();
        }
    }
}