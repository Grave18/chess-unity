using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.ChessBoard.Pieces;
using Chess3D.Runtime.Core.Effects;
using Chess3D.Runtime.Core.Sound;

namespace Chess3D.Runtime.Core.Logic.Moves
{
    public class Capture : Turn
    {
        private readonly Piece _beatenPiece;
        private readonly Turn _simpleMove;
        private readonly EffectsRunner effectsRunner;

        public Capture(Game game, Piece movedPiece, Square fromSquare, Square toSquare, Piece beatenPiece, bool isFirstMove)
        {
            _beatenPiece = beatenPiece;
            _simpleMove = new SimpleMove(game,movedPiece, fromSquare,toSquare, isFirstMove, isComposite: true);

            effectsRunner = _beatenPiece.GetComponent<EffectsRunner>();
        }

        public override void Progress(float t)
        {
            _simpleMove.Progress(t);
            effectsRunner?.ProgressDisappear(t);
        }

        public override void End()
        {
            Square beatenPieceSquare = _beatenPiece.GetSquare();

            _beatenPiece.RemoveFromBoard();
            BeatenPieces.Instance.Add(_beatenPiece, beatenPieceSquare);

            _simpleMove.End();

            effectsRunner?.Appear();
        }

        public override void EndUndo()
        {
            (Piece beatenPiece, Square beatenPieceSquare) = BeatenPieces.Instance.Remove();

            // Order matters
            _simpleMove.EndUndo();
            beatenPiece.AddToBoard(beatenPieceSquare);

            effectsRunner?.Appear();
        }

        public override void PlaySound()
        {
            EffectsPlayer.Instance.PlayCaptureSound();
        }
    }
}