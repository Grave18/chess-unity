using Chess3D.Runtime.ChessBoard;
using Chess3D.Runtime.ChessBoard.Pieces;

namespace Chess3D.Runtime.Logic
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
