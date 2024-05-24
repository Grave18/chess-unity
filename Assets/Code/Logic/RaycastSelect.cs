using UnityEngine;

namespace Logic
{
    public class RaycastSelect : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float maxDistance;
        [SerializeField] private LayerMask layerMask;

        private IHighlightable _previouslyHighlited;
        private ISelectable _previouslySelected;
        private Transform _previouslySelectedTransform;

        private void Update()
        {
            // Cast ray from cursor
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            bool isHit = Physics.Raycast(ray, out var hit, maxDistance, layerMask);
            Transform hitTransform = hit.transform;

            Select(isHit, hitTransform);
            Highlight(isHit, hitTransform);
        }

        private void Select(bool isHit, Transform hitTransform)
        {
            if (!Input.GetButtonDown("Fire1"))
            {
                return;
            }

            if(isHit)
            {
                bool tryGetSelected = hitTransform.TryGetComponent<ISelectable>(out var selectable);

                // Move
                // _previouslySelected?.Move(hitTransform.position);

                // Reselect
                if (tryGetSelected && !selectable.IsEqual(_previouslySelected))
                {
                    selectable.Select();
                    _previouslySelected?.DisableSelect();
                    _previouslySelected = selectable;
                }
            }
            else
            {
                _previouslySelected?.DisableSelect();
                _previouslySelected = null;
            }
        }

        private void Highlight(bool isHit, Transform hitTransform)
        {
            if (isHit)
            {
                bool tryGetHighlightable = hitTransform.TryGetComponent<IHighlightable>(out var highlightable);

                if (tryGetHighlightable && !highlightable.IsEqual(_previouslyHighlited))
                {
                    highlightable.Highlight();
                    _previouslyHighlited?.DisableHighlight();
                    _previouslyHighlited = highlightable;
                }
            }
            else
            {
                _previouslyHighlited?.DisableHighlight();
                _previouslyHighlited = null;
            }
        }
    }
}
