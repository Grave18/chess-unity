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
                // gameManager.Selected?.DisableSelect();
                gameManager.Selected = null;

                return;
            }

            bool tryGetSelectable = hitTransform.TryGetComponent(out ISelectable selectable);

            // Do nothing if select same thing
            if (!tryGetSelectable || selectable.IsEqual(gameManager.Selected))
            {
                return;
            }

            // Move
            if (!selectable.HasPiece())
            {
                Piece piece = gameManager.Selected?.GetPiece();
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

                // gameManager.Selected?.DisableSelect();
                gameManager.Selected = null;

                return;
            }

            // Select
            if (gameManager.Selected == null)
            {
                // selectable.Select();
                gameManager.Selected = selectable;
            }
            // Deselect previously selected and select new
            else if (selectable.GetPieceColor() == gameManager.Selected.GetPieceColor())
            {
                // selectable.Select();
                // gameManager.Selected.DisableSelect();
                gameManager.Selected = selectable;
            }
            // Eat
            else
            {
                Piece piece = gameManager.Selected?.GetPiece();
                Square square = selectable.GetSquare();

                if (piece != null && piece.CanEatAt(square))
                {
                    commandManager.EatAt(piece, square);
                }

                // gameManager.Selected?.DisableSelect();
                gameManager.Selected = null;
            }
        }

        private void Highlight(bool isHit, Transform hitTransform)
        {
            // Dehighlight if not hit anything
            if (!isHit)
            {
                // gameManager.PreviouslyHighlighted?.DisableHighlight();
                gameManager.Highlighted = null;

                return;
            }

            bool tryGetSelectable = hitTransform.TryGetComponent(out ISelectable selectable);

            // Highlight
            if (tryGetSelectable && !selectable.IsEqual(gameManager.Highlighted))
            {
                // selectable.Highlight();
                // gameManager.Highlighted?.DisableHighlight();
                gameManager.Highlighted = selectable;
            }
        }
    }
}
