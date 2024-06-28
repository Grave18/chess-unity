using UnityEngine;
using Board;
using Board.Pieces;
using Logic.Notation;

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
                Click(isHit, hitTransform);
            }
            else
            {
                Hover(isHit, hitTransform);
            }
        }

        private void Click(bool isHit, Transform hitTransform)
        {
            // Deselect if not hit anything
            if (!isHit)
            {
                gameManager.Selected = null;
                return;
            }

            bool tryGetSelectable = hitTransform.TryGetComponent(out ISelectable selectable);

            // Do nothing if select same thing
            if (!tryGetSelectable || selectable.IsEqual(gameManager.Selected))
            {
                return;
            }

            // Cannot select if not right turn
            if (gameManager.Selected == null && selectable.HasPiece() &&
                !gameManager.IsRightTurn(selectable.GetPieceColor()))
            {
                return;
            }

            if (!selectable.HasPiece())
            {
                MakeMove(selectable);
            }
            else if (gameManager.Selected == null ||
                     selectable.GetPieceColor() == gameManager.Selected.GetPieceColor())
            {
                Select(selectable);
            }
            else
            {
                CapturePiece(selectable);
            }
        }

        private void MakeMove(ISelectable selectable)
        {
            Piece piece = gameManager.Selected?.GetPiece();
            Square square = selectable.GetSquare();

            // Move
            if (piece != null && piece.CanMoveTo(square))
            {
                commandManager.MoveTo(piece, square);
                gameManager.Selected = null;
            }
            // Castling
            else if (piece is King king &&
                     king.CanCastlingAt(square, out Rook rook, out Square rookSquare, out TurnType turnType))
            {
                commandManager.Castling(king, square, rook, rookSquare, turnType);
                gameManager.Selected = null;
            }
        }

        /// <summary>
        /// // Select or reselect piece of same color
        /// </summary>
        /// <param name="selectable"></param>
        private void Select(ISelectable selectable)
        {
            gameManager.Selected = selectable;
        }

        private void CapturePiece(ISelectable selectable)
        {
            Piece piece = gameManager.Selected.GetPiece();
            Square square = selectable.GetSquare();

            if (piece.CanEatAt(square))
            {
                commandManager.EatAt(piece, square);
                gameManager.Selected = null;
            }
        }

        private void Hover(bool isHit, Transform hitTransform)
        {
            // Dehighlight if not hit anything
            if (!isHit)
            {
                gameManager.Highlighted = null;

                return;
            }

            bool tryGetSelectable = hitTransform.TryGetComponent(out ISelectable selectable);

            // Highlight
            if (tryGetSelectable && !selectable.IsEqual(gameManager.Highlighted))
            {
                gameManager.Highlighted = selectable;
            }
        }
    }
}
