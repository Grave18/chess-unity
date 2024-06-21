using Board;
using Board.Pieces;

namespace Logic
{
    public interface ISelectable
    {
        PieceColor GetPieceColor();
        Square GetSquare();
        Piece GetPiece();

        bool IsSquare();
        bool HasPiece();
        bool IsEqual(ISelectable other);
    }
}
