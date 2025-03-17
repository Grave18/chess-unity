using System.Threading.Tasks;
using AlgebraicNotation;
using ChessBoard;
using ChessBoard.Pieces;

namespace Logic.CommandPattern
{
    public class CastlingCommand : Command
    {
        private readonly Rook _rook;
        private readonly Square _rookToSquare;
        private readonly NotationTurnType _notationTurnType;

        private Square _previousKingSquare;
        private Square _previousRookSquare;

        public CastlingCommand(King piece, Square kingToSquare, Rook rook, Square rookToSquare, NotationTurnType notationTurnType)
            : base(piece,piece.GetSquare(), kingToSquare, notationTurnType)
        {
            _rook = rook;
            _rookToSquare = rookToSquare;
        }

        public override async Task Execute()
        {
            _previousKingSquare = Piece.GetSquare();
            _previousRookSquare = _rook.GetSquare();

            await Piece.MoveToAsync(MoveToSquare);
            await _rook.MoveToAsync(_rookToSquare);

            Piece.IsFirstMove = false;
            _rook.IsFirstMove = false;
        }

        public override async Task Undo()
        {
            if (_previousKingSquare == null || _previousRookSquare == null)
            {
                return;
            }

            await _rook.MoveToAsync(_previousRookSquare);
            await Piece.MoveToAsync(_previousKingSquare);

            _rook.IsFirstMove = true;
            Piece.IsFirstMove = true;
        }
    }
}
