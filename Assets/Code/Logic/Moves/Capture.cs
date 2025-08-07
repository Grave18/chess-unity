using ChessBoard;
using ChessBoard.Pieces;
using Sound;

namespace Logic.Moves
{
    public class Capture : Turn
    {
        private readonly Piece _beatenPiece;
        private readonly Turn _simpleMove;

        public Capture(Game game, Piece movedPiece, Square fromSquare, Square toSquare, Piece beatenPiece, bool isFirstMove)
        {
            _beatenPiece = beatenPiece;
            _simpleMove = new SimpleMove(game,movedPiece, fromSquare,toSquare, isFirstMove, isComposite: true);
        }

        public override void Progress(float t, bool isUndo = false)
        {
            _simpleMove.Progress(t);
        }

        public override void End()
        {
            Square beatenPieceSquare = _beatenPiece.GetSquare();

            _beatenPiece.RemoveFromBoard();
            BeatenPieces.Instance.Add(_beatenPiece, beatenPieceSquare);

            _simpleMove.End();
        }

        public override void EndUndo()
        {
            (Piece beatenPiece, Square beatenPieceSquare) = BeatenPieces.Instance.Remove();

            // Order matters
            _simpleMove.EndUndo();
            beatenPiece.AddToBoard(beatenPieceSquare);
        }

        public override void PlaySound()
        {
            base.PlaySound();
            EffectsPlayer.Instance.PlayCaptureSound();
        }
    }
}