using Board;
using Board.Pieces;

namespace Logic.CommandPattern
{
    public class EatCommand : Command
    {
        private readonly Piece _piece;
        private readonly Square _square;
        private readonly GameManager _gameManager;
        private readonly SeriesList _seriesList;

        private PieceColor _previousTurn;
        private Square _previousSquare;
        private bool _previousIsFirstMove;
        private Piece _beatenPiece;

        public EatCommand(Piece piece, Square square, GameManager gameManager, SeriesList seriesList)
        {
            _piece = piece;
            _square = square;
            _gameManager = gameManager;
            _seriesList = seriesList;
        }

        public override void Execute()
        {
            _seriesList.AddTurn(_piece, _square, _gameManager.CurrentTurn, TurnType.Capture);

            _previousSquare = _piece.GetSquare();
            _previousIsFirstMove = _piece.IsFirstMove;
            _previousTurn = _gameManager.CurrentTurn;

            _piece.IsFirstMove = false;

            _beatenPiece = _piece.EatAt(_square);
            _piece.MoveTo(_square);

            _gameManager.ChangeTurn();
        }

        public override void Undo()
        {
            if (_previousSquare == null || _beatenPiece == null)
            {
                return;
            }

            _seriesList.RemoveTurn(_previousTurn);

            _piece.MoveTo(_previousSquare);
            _piece.IsFirstMove = _previousIsFirstMove;

            _beatenPiece.RemoveFromBeaten(_square);

            _gameManager.ChangeTurn();
        }
    }
}
