using System;
using UnityEngine;
using Board;
using Board.Pieces;

namespace Logic
{
    public class Selector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CommandManager commandManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Camera mainCamera;

        [Header("Settings")]
        [SerializeField] private float maxDistance;
        [SerializeField] private LayerMask layerMask;

        public event Action OnClick;

        private void Update()
        {
            // Cast ray from cursor
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            bool isHit = Physics.Raycast(ray, out var hit, maxDistance, layerMask);
            Transform hitTransform = hit.transform;

            if (Input.GetButtonDown("Fire1") && gameManager.GameState == GameState.Idle)
            {
                Click(isHit, hitTransform);
                OnClick?.Invoke();
            }
            else
            {
                Hover(isHit, hitTransform);
            }
        }

        private void Click(bool isHit, Transform hitTransform)
        {
            // Deselect if not hit anything or clicked on already selected
            if (IsHitNothingOrAlreadySelected(isHit, hitTransform, out ISelectable selectable))
            {
                DeselectCurrent();
                return;
            }

            // Select if clicked on current turn piece
            if (selectable.HasPiece() && gameManager.IsRightTurn(selectable.GetPieceColor()))
            {
                Select(selectable);
                return;
            }

            // Exit early if no selected piece
            if (gameManager.Selected == null)
            {
                return;
            }

            Piece piece = gameManager.Selected.GetPiece();
            Square moveToSquare = selectable.GetSquare();

            // Move
            if (piece.CanMoveTo(moveToSquare))
            {
                commandManager.MoveTo(piece, moveToSquare);
                DeselectCurrent();
            }
            // Castling
            else if (piece is King king && king.CanCastlingAt(moveToSquare, out CastlingInfo castlingInfo))
            {
                commandManager.Castling(king, moveToSquare, castlingInfo.Rook, castlingInfo.RookSquare, castlingInfo.NotationTurnType);
                DeselectCurrent();
            }
            // Eat
            else if (piece.CanEatAt(moveToSquare, out Piece beatenPiece))
            {
                commandManager.EatAt(piece, beatenPiece, moveToSquare);
                DeselectCurrent();
            }
        }

        private bool IsHitNothingOrAlreadySelected(bool isHit, Transform hitTransform, out ISelectable selectable)
        {
            selectable = null;

            return !isHit
                   || !hitTransform.TryGetComponent(out selectable)
                   || selectable.IsEqual(gameManager.Selected);
        }

        private void Select(ISelectable selectable)
        {
            gameManager.Selected = selectable;
        }

        private void DeselectCurrent()
        {
            gameManager.Selected = null;
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
