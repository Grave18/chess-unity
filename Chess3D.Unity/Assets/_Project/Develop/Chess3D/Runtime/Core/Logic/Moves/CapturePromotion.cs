using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.ChessBoard.Pieces;
using Chess3D.Runtime.Core.Sound;

namespace Chess3D.Runtime.Core.Logic.Moves
{
    public class CapturePromotion : Turn
    {
        private readonly Capture _capture;
        private readonly Promotion _promotion;
        private readonly EffectsPlayer _effectsPlayer;

        public CapturePromotion(Game game, Piece piece, Square fromSquare, Square toSquare, Piece otherPiece,
            Piece beatenPiece, EffectsPlayer effectsPlayer)
        {
            _effectsPlayer = effectsPlayer;
            _capture = new Capture(game, piece, fromSquare, toSquare, beatenPiece, isFirstMove: false, effectsPlayer);
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
            _effectsPlayer.PlayPromoteSound();
        }
    }
}