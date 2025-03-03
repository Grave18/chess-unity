using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Pieces;

namespace Logic.CommandPattern
{
    public abstract class Command
    {
        public string UciMove { get; }

        public abstract Task Execute();
        public abstract Task Undo();
        public abstract Piece GetPiece();

        protected Command(Square from, Square to)
        {
            UciMove = $"{from.Address}{to.Address}";
        }
    }
}