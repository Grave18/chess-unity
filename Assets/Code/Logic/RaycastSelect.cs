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

        private void Select(bool isHit, Transform hitTransform)
        {
            // Deselect if not hit anything
            if (!isHit)
            {
                _previouslySelected?.DisableSelect();
                _previouslySelected = null;
                return;
            }

            bool tryGetSelectable = hitTransform.TryGetComponent(out ISelectable selectable);

            // Do nothing if select same thing
            if (!tryGetSelectable || selectable.IsEqual(_previouslySelected))
            {
                return;
            }

            // Move
            if (!selectable.HasPiece())
            {
                _previouslySelected?.MoveToAndEat(hitTransform.position, selectable);
                _previouslySelected?.DisableSelect();
                _previouslySelected = null;
                return;
            }

            // Select
            if (_previouslySelected == null)
            {
                selectable.Select();
                _previouslySelected = selectable;
            }
            // Deselect previously selected and select new
            else if (selectable.GetPieceColor() == _previouslySelected.GetPieceColor())
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

        private void Highlight(bool isHit, Transform hitTransform)
        {
            // Dehighlight if not hit anything
            if (!isHit)
            {
                _previouslyHighlighted?.DisableHighlight();
                _previouslyHighlighted = null;
                return;
            }

            bool tryGetSelectable = hitTransform.TryGetComponent(out ISelectable selectable);

            // Highlight
            if (tryGetSelectable && !selectable.IsEqual(_previouslyHighlighted))
            {
                selectable.Highlight();
                _previouslyHighlighted?.DisableHighlight();
                _previouslyHighlighted = selectable;
            }
        }
    }
}
