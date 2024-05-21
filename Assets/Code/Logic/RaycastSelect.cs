using UnityEngine;

namespace Logic
{
    public class RaycastSelect : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float maxDistance;
        [SerializeField] private LayerMask layerMask;

        private IHighlightable _previouslyHighlited;

        private void Update()
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            bool isHit = Physics.Raycast(ray, out var hit, maxDistance, layerMask);
            Transform hitTransform = hit.transform;

            // Highlight
            if (isHit)
            {
                bool tryGetHighlighted = hitTransform.TryGetComponent<IHighlightable>(out var highlighted);

                if (tryGetHighlighted && highlighted != _previouslyHighlited)
                {
                    highlighted.Highlight();
                    _previouslyHighlited?.DisableHighlight();
                    _previouslyHighlited = highlighted;
                }
            }
            else
            {
                _previouslyHighlited?.DisableHighlight();
                _previouslyHighlited = null;
            }

            // Select
            if (Input.GetButtonDown("Fire1") && isHit)
            {
                if (hitTransform.TryGetComponent<ISelectable>(out var selected))
                {
                    selected.Select();
                }
            }
        }
    }
}
