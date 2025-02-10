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
            // Deselect if not hit anything
            if (!isHit)
            {
                DeselectCurrent();
                return;
            }

            bool tryGetSelectable = hitTransform.TryGetComponent(out ISelectable selectable);

            // Deselect if clicked on already selected
            if (!tryGetSelectable || selectable.IsEqual(gameManager.Selected))
            {
                DeselectCurrent();
                return;
            }

            // Select if right turn
            if (selectable.HasPiece() && gameManager.IsRightTurn(selectable.GetPieceColor()))
            {
                Select(selectable);
            }
            // Make move if destination square is empty
            else if (gameManager.Selected != null && !selectable.HasPiece())
            {
                MakeMove(selectable);
            }
            else if(gameManager.Selected != null && !gameManager.IsRightTurn(selectable.GetPieceColor()))
            {
                CapturePiece(selectable);
            }
        }

        private void MakeMove(ISelectable selectable)
        {
            Piece piece = gameManager.Selected.GetPiece();
            Square square = selectable.GetSquare();

            // Move
            if (piece.CanMoveTo(square))
            {
                commandManager.MoveTo(piece, square);
                DeselectCurrent();
            }
            // Castling
            else if (piece is King king && king.CanCastlingAt(square, out CastlingInfo castlingInfo))
            {
                commandManager.Castling(king, square, castlingInfo.Rook, castlingInfo.RookSquare, castlingInfo.NotationTurnType);
                DeselectCurrent();
            }
        }

        private void CapturePiece(ISelectable selectable)
        {
            Piece piece = gameManager.Selected.GetPiece();
            Square square = selectable.GetSquare();

            if (piece.CanEatAt(square))
            {
                commandManager.EatAt(piece, square);
                DeselectCurrent();
            }
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
