using Board;
using Board.Pieces;

namespace Logic.CommandPattern
{
    public class EatCommand : Command
    {
        private readonly Piece _piece;
        private readonly Square _square;
        private Square _previousSquare;
        private Piece _beatenPiece;

        public EatCommand(Piece piece, Square square)
        {
            _piece = piece;
            _square = square;
        }

        public override void Execute()
        {
            _previousSquare = _piece.GetSquare();

            // Eat than move
            _beatenPiece = _piece.EatAt(_square);
            _piece.MoveTo(_square);
        }

        public override void Undo()
        {
            if (_previousSquare == null || _beatenPiece == null)
            {
                return;
            }

            _piece.MoveTo(_previousSquare);
            _beatenPiece.RemoveFromBeaten(_square);
            _previousSquare = null;
        }
    }
}
