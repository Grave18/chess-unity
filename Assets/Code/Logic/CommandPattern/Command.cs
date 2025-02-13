using System.Threading.Tasks;
using Board.Pieces;

namespace Logic.CommandPattern
{
    public abstract class Command
    {
        public abstract Task ExecuteAsync();
        public abstract Task UndoAsync();
        public abstract Piece GetPiece();
    }
}
