using System.Threading.Tasks;
using AlgebraicNotation;
using ChessBoard;
using ChessBoard.Info;
using ChessBoard.Pieces;

namespace Logic.CommandPattern
{
    public class MoveCommand : Command
    {
        private Square _previousSquare;
        private bool _previousIsFirstMove;

        public MoveCommand(Piece piece, Square moveToSquare, MoveInfo moveInfo)
            : base(piece, piece.GetSquare(), moveToSquare, NotationTurnType.Move, moveInfo.EnPassantSquare)
        {

        }

        public override async Task Execute()
        {
            // Backup
            _previousSquare = Piece.GetSquare();
            _previousIsFirstMove = Piece.IsFirstMove;
            Piece.IsFirstMove = false;

            // Move
            await Piece.MoveToAsync(MoveToSquare);
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
