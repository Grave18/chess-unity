using System.Threading.Tasks;
using Board;
using Board.Pieces;
using Logic.Notation;

namespace Logic.CommandPattern
{
    public class CastlingCommand : Command
    {
        private readonly King _king;
        private readonly Square _kingSquare;
        private readonly Rook _rook;
        private readonly Square _rookSquare;
        private readonly GameManager _gameManager;
        private readonly SeriesList _seriesList;
        private readonly NotationTurnType _notationTurnType;

        private PieceColor _previousTurn;
        private Square _previousKingSquare;
        private Square _previousRookSquare;

        public CastlingCommand(King king, Square kingSquare, Rook rook, Square rookSquare, GameManager gameManager,
            SeriesList seriesList, NotationTurnType notationTurnType)
        {
            _king = king;
            _kingSquare = kingSquare;
            _rook = rook;
            _rookSquare = rookSquare;
            _gameManager = gameManager;
            _seriesList = seriesList;
            _notationTurnType = notationTurnType;
        }

        public override async Task ExecuteAsync()
        {
            _gameManager.StartTurn();
            _seriesList.AddTurn(null, null, _gameManager.CurrentTurnColor, _notationTurnType);

            _previousKingSquare = _king.GetSquare();
            _previousRookSquare = _rook.GetSquare();
            _previousTurn = _gameManager.CurrentTurnColor;

            await _king.MoveToAsync(_kingSquare);
            await _rook.MoveToAsync(_rookSquare);

            _king.IsFirstMove = false;
            _rook.IsFirstMove = false;

            _gameManager.EndTurn();
        }

        public override async Task UndoAsync()
        {
            if (_previousKingSquare == null || _previousRookSquare == null)
            {
                return;
            }

            _gameManager.StartTurn();

            _seriesList.RemoveTurn(_previousTurn);

            await _rook.MoveToAsync(_previousRookSquare);
            await _king.MoveToAsync(_previousKingSquare);

            _rook.IsFirstMove = true;
            _king.IsFirstMove = true;

            _gameManager.EndTurn();
        }
    }
}
