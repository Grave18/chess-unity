using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Pieces;
using Logic.Notation;

namespace Logic.CommandPattern
{
    public class CastlingCommand : Command
    {
        private readonly King _king;
        private readonly Square _kingSquare;
        private readonly Rook _rook;
        private readonly Square _rookSquare;
        private readonly Game _game;
        private readonly SeriesList _seriesList;
        private readonly NotationTurnType _notationTurnType;

        private PieceColor _previousTurn;
        private Square _previousKingSquare;
        private Square _previousRookSquare;

        public CastlingCommand(King king, Square kingSquare, Rook rook, Square rookSquare, Game game,
            SeriesList seriesList, NotationTurnType notationTurnType)
        {
            _king = king;
            _kingSquare = kingSquare;
            _rook = rook;
            _rookSquare = rookSquare;
            _game = game;
            _seriesList = seriesList;
            _notationTurnType = notationTurnType;
        }

        public override async Task ExecuteAsync()
        {
            _game.StartTurn();
            _seriesList.AddTurn(null, null, _game.CurrentTurnColor, _notationTurnType);

            _previousKingSquare = _king.GetSquare();
            _previousRookSquare = _rook.GetSquare();
            _previousTurn = _game.CurrentTurnColor;

            await _king.MoveToAsync(_kingSquare);
            await _rook.MoveToAsync(_rookSquare);

            _king.IsFirstMove = false;
            _rook.IsFirstMove = false;

            _game.EndTurn();
        }

        public override async Task UndoAsync()
        {
            if (_previousKingSquare == null || _previousRookSquare == null)
            {
                return;
            }

            _game.StartTurn();

            _seriesList.RemoveTurn(_previousTurn);

            await _rook.MoveToAsync(_previousRookSquare);
            await _king.MoveToAsync(_previousKingSquare);

            _rook.IsFirstMove = true;
            _king.IsFirstMove = true;

            _game.EndTurn();
        }

        public override Piece GetPiece()
        {
            return _king;
        }
    }
}
