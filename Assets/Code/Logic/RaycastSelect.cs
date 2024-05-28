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
            if (isHit)
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
            if (isHit)
            {
                bool tryGetSelectable = hitTransform.TryGetComponent(out ISelectable selectable);

                if (!tryGetSelectable || selectable.IsEqual(_previouslySelected))
                {
                    return;
                }

                if (selectable.HasPiece())
                {
                    // Select
                    if(_previouslySelected == null)
                    {
                        selectable.Select();
                        _previouslySelected = selectable;
                    }
                    // Reselect
                    else if(selectable.GetPieceColor() == _previouslySelected.GetPieceColor())
                    {
                        selectable.Select();
                        _previouslySelected.DisableSelect();
                        _previouslySelected = selectable;
                    }
                    // Eat
                    else
                    {
                        _previouslySelected?.MoveToAndEat(hitTransform.position, selectable);
                        _previouslySelected?.DisableSelect();
                        _previouslySelected = null;
                    }
                }
                // Move
                else
                {
                    _previouslySelected?.MoveToAndEat(hitTransform.position, selectable);
                    _previouslySelected?.DisableSelect();
                    _previouslySelected = null;
                }
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
