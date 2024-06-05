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

        void MoveToAndEat(Vector3 position, ISelectable selectable);

        PieceColor GetPieceColor();
        Square GetSquare();
        Piece GetPiece();

        bool IsSquare();
        bool HasPiece();
        bool IsEqual(ISelectable other);
    }
}
