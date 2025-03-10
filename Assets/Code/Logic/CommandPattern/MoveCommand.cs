using System.Threading.Tasks;
using AlgebraicNotation;
using ChessBoard;
using ChessBoard.Info;
using ChessBoard.Pieces;

namespace Logic.CommandPattern
{
    public class MoveCommand : Command
    {
        private readonly Game _game;
        private readonly SeriesList _seriesList;

        private Square _previousSquare;
        private bool _previousIsFirstMove;

        public MoveCommand(Piece piece, Square moveToSquare, MoveInfo moveInfo, Game game, SeriesList seriesList)
            : base(piece, piece.GetSquare(), moveToSquare, moveInfo.Is2SquaresPawnMove)
        {
            _game = game;
            _seriesList = seriesList;
        }

        public override async Task Execute()
        {
            // Backup
            _previousSquare = Piece.GetSquare();
            _previousIsFirstMove = Piece.IsFirstMove;
            Piece.IsFirstMove = false;

            // Move
            await Piece.MoveToAsync(MoveToSquare);

            // _seriesList.AddTurn(_piece, _moveToSquare, _game.PreviousTurnColor, NotationTurnType.Move, _game.CheckType);
        }

        public override async Task Undo()
        {
            if (_previousSquare == null)
            {
                return;
            }

            // Move back
            await Piece.MoveToAsync(_previousSquare);
            Piece.IsFirstMove = _previousIsFirstMove;

            // _seriesList.RemoveTurn(_game.CurrentTurnColor);
        }
    }
}
