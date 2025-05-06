using ChessGame.ChessBoard;
using ChessGame.ChessBoard.Pieces;

namespace ChessGame.Logic
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
