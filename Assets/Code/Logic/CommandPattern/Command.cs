using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Pieces;
using Logic.MovesBuffer;

namespace Logic.CommandPattern
{
    public abstract class Command
    {
        public Piece Piece { get; protected set; }
        public Square MoveFromSquare { get; }
        public Square MoveToSquare { get; }
        public bool IsUndoable { get; }
        public Piece PromotedPiece { get; protected set; }
        public Square EnPassantSquare { get; }
        public MoveType MoveType { get; }

        public abstract Task Execute();
        public abstract Task Undo();

        protected Command(Piece piece, Square from, Square to, MoveType moveType, Square enPassantSquare = null, bool isUndoable = true)
        {
            Piece = piece;
            MoveFromSquare = from;
            MoveToSquare = to;
            MoveType = moveType;
            EnPassantSquare = enPassantSquare;
            IsUndoable = isUndoable;
        }

        public string GetUciMove()
        {
            if (MoveFromSquare == null || MoveToSquare == null)
            {
                return string.Empty;
            }

            string move = $"{MoveFromSquare.Address}{MoveToSquare.Address}";

            if (PromotedPiece != null)
            {
                move += GetPromotedPieceLetter();
            }

            return move;
        }

        private string GetPromotedPieceLetter()
        {
            return PromotedPiece switch
            {
                Queen => "q",
                Rook => "r",
                Bishop => "b",
                Knight => "n",
                _ => string.Empty,
            };
        }
    }
}