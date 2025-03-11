using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Pieces;

namespace Logic.CommandPattern
{
    public abstract class Command
    {
        public Piece Piece { get; protected set; }
        public Square MoveToSquare { get;}
        public string UciMove { get; protected set; }
        public bool IsUndoable { get;}
        public Square EnPassantSquare { get; }

        public abstract Task Execute();
        public abstract Task Undo();

        protected Command(Piece piece, Square from, Square to, Square enPassantSquare = null, bool isUndoable = true)
        {
            Piece = piece;
            MoveToSquare = to;
            EnPassantSquare = enPassantSquare;
            IsUndoable = isUndoable;

            if (from != null && to != null)
            {
                UciMove = $"{from.Address}{to.Address}";
            }
        }
    }
}