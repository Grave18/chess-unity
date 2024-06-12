using Board;
using Board.Pieces;

namespace Logic.CommandPattern
{
    public class MoveCommand : Command
    {
        private readonly Piece _piece;
        private readonly Square _square;
        private readonly PieceColor _turn;
        private readonly SeriesList _seriesList;

        private Square _previousSquare;

        public MoveCommand(Piece piece, Square square, PieceColor turn, SeriesList seriesList)
        {
            _piece = piece;
            _square = square;
            _turn = turn;
            _seriesList = seriesList;
        }

        public override void Execute()
        {
            _seriesList.AddTurn(_piece, _square, _turn, TurnType.Move);

            _previousSquare = _piece.GetSquare();
            _piece.MoveTo(_square);
        }

        public override void Undo()
        {
            if (_previousSquare == null)
            {
                return;
            }

            _seriesList.RemoveTurn(_turn);

            _piece.MoveTo(_previousSquare);
            _previousSquare = null;
        }
    }
}
