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

        private PieceColor _previousTurn;
        private Square _previousSquare;
        private bool _previousIsFirstMove;

        public EatCommand(Piece piece, Piece beatenPiece, Square moveToSquare, Game game, SeriesList seriesList)
        {
            _piece = piece;
            _beatenPiece = beatenPiece;
            _beatenPieceSquare = _beatenPiece.GetSquare();
            _moveToSquare = moveToSquare;
            _game = game;
            _seriesList = seriesList;
        }

        public override async Task ExecuteAsync()
        {
            _game.StartTurn();
            _seriesList.AddTurn(_piece, _moveToSquare, _game.CurrentTurnColor, NotationTurnType.Capture);

            _previousSquare = _piece.GetSquare();
            _previousIsFirstMove = _piece.IsFirstMove;
            _previousTurn = _game.CurrentTurnColor;

            _piece.IsFirstMove = false;

            _beatenPiece.MoveToBeaten();
            await _piece.MoveToAsync(_moveToSquare);

            _game.EndTurn();
        }

        public override async Task UndoAsync()
        {
            if (_previousSquare == null || _beatenPiece == null)
            {
                return;
            }

            _game.StartTurn();
            _seriesList.RemoveTurn(_previousTurn);

            await _piece.MoveToAsync(_previousSquare);
            _piece.IsFirstMove = _previousIsFirstMove;

            _beatenPiece.RemoveFromBeaten(_beatenPieceSquare);

            _game.EndTurn();
        }

        public override Piece GetPiece()
        {
            return _piece;
        }
    }
}
