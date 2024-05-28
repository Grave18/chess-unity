using Board;
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
        Section GetSection();
        Piece GetPiece();

        bool IsSection();
        bool IsEqual(ISelectable other);
        bool HasPiece();
    }
}
