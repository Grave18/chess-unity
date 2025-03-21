using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Pieces;
using Logic.MovesBuffer;

namespace Logic.CommandPattern
{
    public class EatCommand : Command
    {
        private readonly Piece _beatenPiece;
        private readonly Square _beatenPieceSquare;

        private Square _previousSquare;
        private bool _previousIsFirstMove;

        public EatCommand(Piece piece, Piece beatenPiece, Square moveToSquare, MoveType moveType)
            : base(piece,piece.GetSquare(), moveToSquare, moveType)
        {
            _beatenPiece = beatenPiece;
            _beatenPieceSquare = _beatenPiece.GetSquare();
        }

        public override async Task Execute()
        {
            // Backup
            _previousSquare = Piece.GetSquare();
            _previousIsFirstMove = Piece.IsFirstMove;
            Piece.IsFirstMove = false;

            // Move to beaten
            _beatenPiece.RemoveFromBoard();

            // Move selected piece
            await Piece.MoveToAsync(MoveToSquare);
        }

        public override async Task Undo()
        {
            if (_previousSquare == null || _beatenPiece == null)
            {
                return;
            }

            // Move selected piece
            await Piece.MoveToAsync(_previousSquare);
            Piece.IsFirstMove = _previousIsFirstMove;

            // Add beaten piece to board
            _beatenPiece.AddToBoard(_beatenPieceSquare);
        }
    }
}
