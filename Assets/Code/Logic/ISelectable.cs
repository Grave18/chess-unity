using UnityEngine;

namespace Logic
{
    public interface ISelectable
    {
        void Select();
        void DisableSelect();
        void Highlight();
        void DisableHighlight();

        void Move(Vector3 position, ISelectable selectable);

        bool IsEmpty();
        bool IsEqual(ISelectable other);
    }
}
