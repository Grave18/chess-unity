using ChessGame.ChessBoard;
using ChessGame.ChessBoard.Pieces;

namespace ChessGame.Logic.Moves
{
    public class CapturePromotion : Turn
    {
        private readonly Capture _capture;
        private readonly Promotion _promotion;

        public CapturePromotion(Piece piece, Square fromSquare, Square toSquare, Piece otherPiece, Piece beatenPiece)
        {
            _capture = new Capture(piece, fromSquare, toSquare, beatenPiece, isFirstMove: false);
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
    }
}