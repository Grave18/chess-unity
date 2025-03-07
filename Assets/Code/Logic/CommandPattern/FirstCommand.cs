using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Pieces;

namespace Logic.CommandPattern
{
    /// Not real move. Just contains info about en passant
    public class FirstCommand : Command
    {
        public FirstCommand(Piece pawn, Square toSquare) : base(pawn, from: null, toSquare, is2SquaresPawnMove: true, isUndoable: false)
        {
            // Empty because it's not real move
            UciMove = string.Empty;
        }

        public override Task Execute()
        {
            return Task.CompletedTask;
        }

        public override Task Undo()
        {
            return Task.CompletedTask;
        }
    }
}