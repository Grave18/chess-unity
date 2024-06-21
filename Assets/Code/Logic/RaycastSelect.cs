using UnityEngine;
using Board;
using Board.Pieces;

namespace Logic
{
    public class RaycastSelect : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CommandManager commandManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Camera mainCamera;

        [Header("Settings")]
        [SerializeField] private float maxDistance;
        [SerializeField] private LayerMask layerMask;

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
                gameManager.PreviouslySelected?.DisableSelect();
                gameManager.PreviouslySelected = null;

                return;
            }

            bool tryGetSelectable = hitTransform.TryGetComponent(out ISelectable selectable);

            // Do nothing if select same thing
            if (!tryGetSelectable || selectable.IsEqual(gameManager.PreviouslySelected))
            {
                return;
            }

            // Move
            if (!selectable.HasPiece())
            {
                Piece piece = gameManager.PreviouslySelected?.GetPiece();
                Square square = selectable.GetSquare();

                if (piece != null && piece.CanMoveTo(square))
                {
                    commandManager.MoveTo(piece, square);
                }

                // Castling
                if (piece is King king && king.CanCastlingAt(square, out Rook rook, out Square rookSquare, out TurnType turnType))
                {
                    commandManager.Castling(king, square, rook, rookSquare, turnType);
                }

                gameManager.PreviouslySelected?.DisableSelect();
                gameManager.PreviouslySelected = null;

                return;
            }

            // Select
            if (gameManager.PreviouslySelected == null)
            {
                selectable.Select();
                gameManager.PreviouslySelected = selectable;
            }
            // Deselect previously selected and select new
            else if (selectable.GetPieceColor() == gameManager.PreviouslySelected.GetPieceColor())
            {
                selectable.Select();
                gameManager.PreviouslySelected.DisableSelect();
                gameManager.PreviouslySelected = selectable;
            }
            // Eat
            else
            {
                Piece piece = gameManager.PreviouslySelected?.GetPiece();
                Square square = selectable.GetSquare();

                if (piece != null && piece.CanEatAt(square))
                {
                    commandManager.EatAt(piece, square);
                }

                gameManager.PreviouslySelected?.DisableSelect();
                gameManager.PreviouslySelected = null;
            }
        }

        private void Highlight(bool isHit, Transform hitTransform)
        {
            // Dehighlight if not hit anything
            if (!isHit)
            {
                gameManager.PreviouslyHighlighted?.DisableHighlight();
                gameManager.PreviouslyHighlighted = null;

                return;
            }

            bool tryGetSelectable = hitTransform.TryGetComponent(out ISelectable selectable);

            // Highlight
            if (tryGetSelectable && !selectable.IsEqual(gameManager.PreviouslyHighlighted))
            {
                selectable.Highlight();
                gameManager.PreviouslyHighlighted?.DisableHighlight();
                gameManager.PreviouslyHighlighted = selectable;
            }
        }
    }
}
