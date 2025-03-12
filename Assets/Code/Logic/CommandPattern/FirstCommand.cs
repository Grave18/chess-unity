using System.Threading.Tasks;
using ChessBoard.Info;

namespace Logic.CommandPattern
{
    /// Not real move. Just contains info about en passant
    public class FirstCommand : Command
    {
        public FirstCommand(EnPassantInfo enPassantInfo)
            : base(enPassantInfo.Piece, from: null, to: null, enPassantInfo.Square, isUndoable: false)
        {

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