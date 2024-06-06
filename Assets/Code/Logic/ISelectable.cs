using Board;
using Board.Pieces;
using UnityEngine;

namespace Logic
{
    public interface ISelectable
    {
        void Select();
        void DisableSelect();
        void Highlight();
        void DisableHighlight();

        PieceColor GetPieceColor();
        Square GetSquare();
        Piece GetPiece();

        bool IsSquare();
        bool HasPiece();
        bool IsEqual(ISelectable other);
    }
}
