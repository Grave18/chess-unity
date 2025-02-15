using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Pieces;
using Logic.Notation;

namespace Logic.CommandPattern
{
    public class EatCommand : Command
    {
        private readonly Piece _piece;
        private readonly Piece _beatenPiece;
        private readonly Square _moveToSquare;
        private readonly Square _beatenPieceSquare;
        private readonly Game _game;
        private readonly SeriesList _seriesList;
        private readonly NotationTurnType _notationTurnType;

        private Square _previousSquare;
        private bool _previousIsFirstMove;

        public EatCommand(Piece piece, Piece beatenPiece, Square moveToSquare, Game game, SeriesList seriesList, NotationTurnType notationTurnType)
        {
            _piece = piece;
            _beatenPiece = beatenPiece;
            _beatenPieceSquare = _beatenPiece.GetSquare();
            _moveToSquare = moveToSquare;
            _game = game;
            _seriesList = seriesList;
            _notationTurnType = notationTurnType;
        }

        public override async Task ExecuteAsync()
        {
            _game.StartTurn();

            // Backup
            _previousSquare = _piece.GetSquare();
            _previousIsFirstMove = _piece.IsFirstMove;
            _piece.IsFirstMove = false;

            // Move to beaten
            _beatenPiece.MoveToBeaten();

            // Move selected piece
            await _piece.MoveToAsync(_moveToSquare);

            // End turn and add to notation
            _game.EndTurn();
            _seriesList.AddTurn(_piece, _moveToSquare, _game.PreviousTurnColor, _notationTurnType, _game.CheckType);
        }

        public override async Task UndoAsync()
        {
            if (_previousSquare == null || _beatenPiece == null)
            {
                return;
            }

            _game.StartTurn();

            // Move selected piece
            await _piece.MoveToAsync(_previousSquare);
            _piece.IsFirstMove = _previousIsFirstMove;

            // Add beaten piece to board
            _beatenPiece.RemoveFromBeaten(_beatenPieceSquare);

            // Remove from notation and end turn
            _game.EndTurn();
            _seriesList.RemoveTurn(_game.CurrentTurnColor);
        }

        public override Piece GetPiece()
        {
            return _piece;
        }
    }
}
