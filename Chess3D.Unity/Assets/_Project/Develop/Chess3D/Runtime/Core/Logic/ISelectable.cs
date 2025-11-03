using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.ChessBoard.Pieces;

namespace Chess3D.Runtime.Core.Logic
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
