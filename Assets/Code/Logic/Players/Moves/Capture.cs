using ChessBoard;
using ChessBoard.Pieces;

namespace Logic.Players.Moves
{
    public class Capture : Turn
    {
        private readonly Piece _beatenPiece;
        private readonly Turn _simpleMove;

        public Capture(Piece movedPiece, Square fromSquare, Square toSquare, Piece beatenPiece, bool isFirstMove)
        {
            _beatenPiece = beatenPiece;
            _simpleMove = new SimpleMove(movedPiece, fromSquare,toSquare, isFirstMove);
        }

        public override void Progress(float t)
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
    }
}