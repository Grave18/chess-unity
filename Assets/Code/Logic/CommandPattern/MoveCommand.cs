using System.Threading.Tasks;
using Board;
using Board.Pieces;
using Logic.Notation;

namespace Logic.CommandPattern
{
    public class MoveCommand : Command
    {
        private readonly Piece _piece;
        private readonly Square _square;
        private readonly GameManager _gameManager;
        private readonly SeriesList _seriesList;

        private PieceColor _previousTurn;
        private Square _previousSquare;
        private bool _previousIsFirstMove;

        public MoveCommand(Piece piece, Square square, GameManager gameManager, SeriesList seriesList)
        {
            _piece = piece;
            _square = square;
            _gameManager = gameManager;
            _seriesList = seriesList;
        }

        public override async Task ExecuteAsync()
        {
            _previousSquare = _piece.GetSquare();
            _previousIsFirstMove = _piece.IsFirstMove;
            _previousTurn = _gameManager.CurrentTurnColor;

            _piece.IsFirstMove = false;

            await _piece.MoveToAsync(_square);

            _gameManager.EndTurn();

            // Is it Check?
            NotationTurnType notationTurnType = _gameManager.CheckType switch
                {
                    CheckType.Check => NotationTurnType.Check,
                    CheckType.DoubleCheck => NotationTurnType.DoubleCheck,
                    CheckType.CheckMate => NotationTurnType.CheckMate,
                    _ => NotationTurnType.Move
                };

            _seriesList.AddTurn(_piece, _square, _previousTurn, notationTurnType);
        }

        public override async Task UndoAsync()
        {
            if (_previousSquare == null)
            {
                return;
            }

            _seriesList.RemoveTurn(_previousTurn);

            await _piece.MoveToAsync(_previousSquare);

            _piece.IsFirstMove = _previousIsFirstMove;

            _gameManager.EndTurn();
        }
    }
}
