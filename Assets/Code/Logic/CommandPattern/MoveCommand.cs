using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Pieces;
using Logic.Notation;

namespace Logic.CommandPattern
{
    public class MoveCommand : Command
    {
        private readonly Piece _piece;
        private readonly Square _square;
        private readonly Game _game;
        private readonly SeriesList _seriesList;

        private PieceColor _previousTurn;
        private Square _previousSquare;
        private bool _previousIsFirstMove;

        public MoveCommand(Piece piece, Square square, Game game, SeriesList seriesList)
        {
            _piece = piece;
            _square = square;
            _game = game;
            _seriesList = seriesList;
        }

        public override async Task ExecuteAsync()
        {
            _game.StartTurn();

            _previousSquare = _piece.GetSquare();
            _previousIsFirstMove = _piece.IsFirstMove;
            _previousTurn = _game.CurrentTurnColor;
            _piece.IsFirstMove = false;

            await _piece.MoveToAsync(_square);

            _game.EndTurn();

            // Is it Check?
            NotationTurnType notationTurnType = _game.CheckType switch
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

            _game.StartTurn();

            _seriesList.RemoveTurn(_previousTurn);

            await _piece.MoveToAsync(_previousSquare);

            _piece.IsFirstMove = _previousIsFirstMove;

            _game.EndTurn();
        }

        public override Piece GetPiece()
        {
            return _piece;
        }
    }
}
