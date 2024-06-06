using Board;
using Board.Pieces;

namespace Logic.CommandPattern
{
    public class MoveCommand : Command
    {
        private readonly Piece _piece;
        private readonly Square _square;
        private Square _previousSquare;

        public MoveCommand(Piece piece, Square square)
        {
            _piece = piece;
            _square = square;
        }

        public override void Execute()
        {
            _previousSquare = _piece.GetSquare();

            _piece.MoveTo(_square);
        }

        public override void Undo()
        {
            if (_previousSquare == null)
            {
                return;
            }

            _piece.MoveTo(_previousSquare);
            _previousSquare = null;
        }
    }
}
