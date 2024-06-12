using Board;
using Board.Pieces;

namespace Logic.CommandPattern
{
    public class EatCommand : Command
    {
        private readonly Piece _piece;
        private readonly Square _square;
        private readonly PieceColor _turn;
        private readonly SeriesList _seriesList;

        private Square _previousSquare;
        private Piece _beatenPiece;

        public EatCommand(Piece piece, Square square, PieceColor turn, SeriesList seriesList)
        {
            _piece = piece;
            _square = square;
            _turn = turn;
            _seriesList = seriesList;
        }

        public override void Execute()
        {
            _previousSquare = _piece.GetSquare();
            _seriesList.AddTurn(_piece, _square, _turn, TurnType.Capture);

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

            _seriesList.RemoveTurn(_turn);

            _piece.MoveTo(_previousSquare);
            _beatenPiece.RemoveFromBeaten(_square);
            _previousSquare = null;
        }
    }
}
