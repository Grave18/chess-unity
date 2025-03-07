using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Pieces;
using Logic.Notation;

namespace Logic.CommandPattern
{
    public class EatCommand : Command
    {
        private readonly Piece _beatenPiece;
        private readonly Square _beatenPieceSquare;
        private readonly Game _game;
        private readonly SeriesList _seriesList;
        private readonly NotationTurnType _notationTurnType;

        private Square _previousSquare;
        private bool _previousIsFirstMove;

        public EatCommand(Piece piece, Piece beatenPiece, Square moveToSquare, Game game, SeriesList seriesList,
            NotationTurnType notationTurnType) : base(piece,piece.GetSquare(), moveToSquare)
        {
            _beatenPiece = beatenPiece;
            _beatenPieceSquare = _beatenPiece.GetSquare();
            _game = game;
            _seriesList = seriesList;
            _notationTurnType = notationTurnType;
        }

        public override async Task Execute()
        {
            // Backup
            _previousSquare = Piece.GetSquare();
            _previousIsFirstMove = Piece.IsFirstMove;
            Piece.IsFirstMove = false;

            // Move to beaten
            _beatenPiece.MoveToBeaten();

            // Move selected piece
            await Piece.MoveToAsync(MoveToSquare);

            // _seriesList.AddTurn(_piece, _moveToSquare, _game.PreviousTurnColor, _notationTurnType, _game.CheckType);
        }

        public override async Task Undo()
        {
            if (_previousSquare == null || _beatenPiece == null)
            {
                return;
            }

            // Move selected piece
            await Piece.MoveToAsync(_previousSquare);
            Piece.IsFirstMove = _previousIsFirstMove;

            // Add beaten piece to board
            _beatenPiece.RemoveFromBeaten(_beatenPieceSquare);

            // _seriesList.RemoveTurn(_game.CurrentTurnColor);
        }
    }
}
