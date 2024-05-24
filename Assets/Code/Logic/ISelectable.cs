using UnityEngine;

namespace Logic
{
    public interface ISelectable
    {
        void Select();
        void DisableSelect();

        void Move(Vector3 target);

        bool IsEqual(ISelectable other);
    }
}
