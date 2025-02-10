using System.Threading.Tasks;
using Board;
using Board.Pieces;
using Logic.Notation;

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

        public override async Task ExecuteAsync()
        {
            _gameManager.StartTurn();
            _seriesList.AddTurn(_piece, _square, _gameManager.CurrentTurnColor, NotationTurnType.Capture);

            _previousSquare = _piece.GetSquare();
            _previousIsFirstMove = _piece.IsFirstMove;
            _previousTurn = _gameManager.CurrentTurnColor;

            _piece.IsFirstMove = false;

            _beatenPiece = _piece.EatAt(_square);
            await _piece.MoveToAsync(_square);

            _gameManager.EndTurn();
        }

        public override async Task UndoAsync()
        {
            if (_previousSquare == null || _beatenPiece == null)
            {
                return;
            }

            _gameManager.StartTurn();
            _seriesList.RemoveTurn(_previousTurn);

            await _piece.MoveToAsync(_previousSquare);
            _piece.IsFirstMove = _previousIsFirstMove;

            _beatenPiece.RemoveFromBeaten(_square);

            _gameManager.EndTurn();
        }
    }
}
