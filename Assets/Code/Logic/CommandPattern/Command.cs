using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Pieces;

namespace Logic.CommandPattern
{
    public abstract class Command
    {
        public string UciMove { get; private set; }

        public abstract Task ExecuteAsync();
        public abstract Task UndoAsync();
        public abstract Piece GetPiece();

        protected Command(Square from, Square to)
        {
            UciMove = $"{from.Address}{to.Address}";
        }
    }
}