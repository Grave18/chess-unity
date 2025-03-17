using System.Threading.Tasks;
using AlgebraicNotation;
using ChessBoard;
using ChessBoard.Pieces;

namespace Logic.CommandPattern
{
    public class EatCommand : Command
    {
        private readonly Piece _beatenPiece;
        private readonly Square _beatenPieceSquare;

        private Square _previousSquare;
        private bool _previousIsFirstMove;

        public EatCommand(Piece piece, Piece beatenPiece, Square moveToSquare, NotationTurnType notationTurnType)
            : base(piece,piece.GetSquare(), moveToSquare, notationTurnType)
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
            _beatenPiece.MoveToBeaten();

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
            _beatenPiece.RemoveFromBeaten(_beatenPieceSquare);
        }
    }
}
