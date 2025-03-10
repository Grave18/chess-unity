using System.Threading.Tasks;
using AlgebraicNotation;
using ChessBoard;
using ChessBoard.Pieces;

namespace Logic.CommandPattern
{
    public class CastlingCommand : Command
    {
        private readonly Rook _rook;
        private readonly Square _rookToSquare;
        private readonly Game _game;
        private readonly SeriesList _seriesList;
        private readonly NotationTurnType _notationTurnType;

        private Square _previousKingSquare;
        private Square _previousRookSquare;

        public CastlingCommand(King piece, Square kingToSquare, Rook rook, Square rookToSquare, Game game,
            SeriesList seriesList, NotationTurnType notationTurnType) : base(piece,piece.GetSquare(), kingToSquare)
        {
            _rook = rook;
            _rookToSquare = rookToSquare;
            _game = game;
            _seriesList = seriesList;
            _notationTurnType = notationTurnType;
        }

        public override async Task Execute()
        {
            _previousKingSquare = Piece.GetSquare();
            _previousRookSquare = _rook.GetSquare();

            await Piece.MoveToAsync(MoveToSquare);
            await _rook.MoveToAsync(_rookToSquare);

            Piece.IsFirstMove = false;
            _rook.IsFirstMove = false;

            // _seriesList.AddTurn(null, null, _game.PreviousTurnColor, _notationTurnType, _game.CheckType);
        }

        public override async Task Undo()
        {
            if (_previousKingSquare == null || _previousRookSquare == null)
            {
                return;
            }

            await _rook.MoveToAsync(_previousRookSquare);
            await Piece.MoveToAsync(_previousKingSquare);

            _rook.IsFirstMove = true;
            Piece.IsFirstMove = true;

            // _seriesList.RemoveTurn(_game.CurrentTurnColor);
        }
    }
}
