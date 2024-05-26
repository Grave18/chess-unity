using UnityEngine;

namespace Logic
{
    public class RaycastSelect : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float maxDistance;
        [SerializeField] private LayerMask layerMask;

        private ISelectable _previouslySelected;
        private ISelectable _previouslyHighlighted;

        private void Update()
        {
            // Cast ray from cursor
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            bool isHit = Physics.Raycast(ray, out var hit, maxDistance, layerMask);
            Transform hitTransform = hit.transform;

            if (Input.GetButtonDown("Fire1"))
            {
                Select(isHit, hitTransform);
            }
            else
            {
                Highlight(isHit, hitTransform);
            }
        }

        private void Highlight(bool isHit, Transform hitTransform)
        {
            if(isHit)
            {
                bool tryGetSelectable = hitTransform.TryGetComponent(out ISelectable selectable);

                // Highlight
                if (tryGetSelectable && !selectable.IsEqual(_previouslyHighlighted))
                {
                    selectable.Highlight();
                    _previouslyHighlighted?.DisableHighlight();
                    _previouslyHighlighted = selectable;
                }
            }
            // Dehighlight if not hit anything
            else
            {
                _previouslyHighlighted?.DisableHighlight();
                _previouslyHighlighted = null;
            }
        }

        private void Select(bool isHit, Transform hitTransform)
        {
            if(isHit)
            {
                bool tryGetSelectable = hitTransform.TryGetComponent(out ISelectable selectable);

                // Move
                if (tryGetSelectable && selectable.IsEmpty())
                {
                    _previouslySelected?.Move(hitTransform.position, selectable);
                    _previouslySelected = null;
                }
                // Reselect
                else if (tryGetSelectable && !selectable.IsEqual(_previouslySelected))
                {
                    selectable.Select();
                    _previouslySelected?.DisableSelect();
                    _previouslySelected = selectable;
                }
                // Deselect if not hit anything
                // else
                // {
                //     _previouslySelected?.DisableSelect();
                //     _previouslySelected = null;
                // }
            }
            // Deselect if not hit anything
            else
            {
                _previouslySelected?.DisableSelect();
                _previouslySelected = null;
            }
        }
    }
}
