using System;
using ChessBoard;
using ChessBoard.Pieces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Logic
{
    public class Selector : MonoBehaviour
    {
        [Header("References")]
        [FormerlySerializedAs("commandManager")]
        [SerializeField] private CommandInvoker commandInvoker;
        [FormerlySerializedAs("gameManager")]
        [SerializeField] private Game game;
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

            if (Input.GetButtonDown("Fire1") && game.GameState == GameState.Idle)
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
            if (selectable.HasPiece() && game.IsRightTurn(selectable.GetPieceColor()))
            {
                Select(selectable);
                return;
            }

            // Exit early if no selected piece
            if (game.Selected == null)
            {
                return;
            }

            Piece piece = game.Selected.GetPiece();
            Square moveToSquare = selectable.GetSquare();

            // Move
            if (piece.CanMoveTo(moveToSquare))
            {
                commandInvoker.MoveTo(piece, moveToSquare);
                DeselectCurrent();
            }
            // Castling
            else if (piece is King king && king.CanCastlingAt(moveToSquare, out CastlingInfo castlingInfo))
            {
                commandInvoker.Castling(king, moveToSquare, castlingInfo.Rook, castlingInfo.RookSquare, castlingInfo.NotationTurnType);
                DeselectCurrent();
            }
            // Eat
            else if (piece.CanEatAt(moveToSquare, out Piece beatenPiece))
            {
                commandInvoker.EatAt(piece, beatenPiece, moveToSquare);
                DeselectCurrent();
            }
        }

        private bool IsHitNothingOrAlreadySelected(bool isHit, Transform hitTransform, out ISelectable selectable)
        {
            selectable = null;

            return !isHit
                   || !hitTransform.TryGetComponent(out selectable)
                   || selectable.IsEqual(game.Selected);
        }

        private void Select(ISelectable selectable)
        {
            game.Selected = selectable;
        }

        private void DeselectCurrent()
        {
            game.Selected = null;
        }

        private void Hover(bool isHit, Transform hitTransform)
        {
            // Dehighlight if not hit anything
            if (!isHit)
            {
                game.Highlighted = null;

                return;
            }

            bool tryGetSelectable = hitTransform.TryGetComponent(out ISelectable selectable);

            // Highlight
            if (tryGetSelectable && !selectable.IsEqual(game.Highlighted))
            {
                game.Highlighted = selectable;
            }
        }
    }
}
