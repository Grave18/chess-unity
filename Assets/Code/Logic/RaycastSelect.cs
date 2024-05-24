using Board;
using UnityEngine;

namespace Logic
{
    public class RaycastSelect : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float maxDistance;
        [SerializeField] private LayerMask layerMask;

        private Piece _previouslySelectedPiece;

        private void Update()
        {
            // Cast ray from cursor
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            bool isHit = Physics.Raycast(ray, out var hit, maxDistance, layerMask);
            Transform hitTransform = hit.transform;

            Select(isHit, hitTransform);
        }

        private void Select(bool isHit, Transform hitTransform)
        {
            if (!Input.GetButtonDown("Fire1"))
            {
                return;
            }

            if(isHit)
            {
                bool tryGetSection = hitTransform.TryGetComponent<Section>(out var section);
                bool tryGetPiece = hitTransform.TryGetComponent<Piece>(out var piece);

                // Move
                if (tryGetSection && section.IsEmpty() && _previouslySelectedPiece != null)
                {
                    _previouslySelectedPiece.Move(hitTransform.position, section);
                    _previouslySelectedPiece.DisableSelect();
                    _previouslySelectedPiece = null;
                }
                // Reselect
                else if (tryGetPiece && piece != _previouslySelectedPiece)
                {
                    piece.Select();
                    _previouslySelectedPiece?.DisableSelect();
                    _previouslySelectedPiece = piece;
                }
                // Deselect if not hit anything
                else
                {
                    _previouslySelectedPiece?.DisableSelect();
                    _previouslySelectedPiece = null;
                }
            }
            // Deselect if not hit anything
            else
            {
                _previouslySelectedPiece?.DisableSelect();
                _previouslySelectedPiece = null;
            }
        }
    }
}
