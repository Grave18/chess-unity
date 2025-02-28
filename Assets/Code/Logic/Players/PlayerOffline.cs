using System;
using ChessBoard;
using ChessBoard.Pieces;
using UnityEngine;

namespace Logic.Players
{
    public class PlayerOffline : Player
    {
        [Header("PlayerOffline")]
        [SerializeField] private Camera mainCamera;

        [Space]
        [SerializeField] private float maxDistance;
        [SerializeField] private LayerMask layerMask;

        public event Action OnClick;

        public bool _isAllowMove;

        public override void AllowMakeMove()
        {
            _isAllowMove = true;
        }

        public override void DisallowMakeMove()
        {
            _isAllowMove = false;
        }

        private void Update()
        {
            if(!_isAllowMove)
            {
                return;
            }

            // Cast ray from cursor
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            bool isHit = Physics.Raycast(ray, out var hit, maxDistance, layerMask);
            Transform hitTransform = hit.transform;

            if (Input.GetButtonDown("Fire1") && Game.State == GameState.Idle)
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
            if (selectable.HasPiece() && Game.IsRightTurn(selectable.GetPieceColor()))
            {
                Select(selectable);
                return;
            }

            // Exit early if no selected piece
            if (Game.Selected == null)
            {
                return;
            }

            Piece piece = Game.Selected.GetPiece();
            Square moveToSquare = selectable.GetSquare();

            // Move
            if (piece.CanMoveTo(moveToSquare))
            {
                CommandInvoker.MoveTo(piece, moveToSquare);
                DeselectCurrent();
            }
            // Castling
            else if (piece is King king && king.CanCastlingAt(moveToSquare, out CastlingInfo castlingInfo))
            {
                CommandInvoker.Castling(king, moveToSquare, castlingInfo.Rook, castlingInfo.RookSquare, castlingInfo.NotationTurnType);
                DeselectCurrent();
            }
            // Eat
            else if (piece.CanEatAt(moveToSquare, out CaptureInfo captureInfo))
            {
                CommandInvoker.EatAt(piece, moveToSquare, captureInfo);
                DeselectCurrent();
            }
        }

        private bool IsHitNothingOrAlreadySelected(bool isHit, Transform hitTransform, out ISelectable selectable)
        {
            selectable = null;

            return !isHit
                   || !hitTransform.TryGetComponent(out selectable)
                   || selectable.IsEqual(Game.Selected);
        }

        private void Select(ISelectable selectable)
        {
            Game.Selected = selectable;
        }

        private void DeselectCurrent()
        {
            Game.Selected = null;
        }

        private void Hover(bool isHit, Transform hitTransform)
        {
            // Dehighlight if not hit anything
            if (!isHit)
            {
                Game.Highlighted = null;

                return;
            }

            bool tryGetSelectable = hitTransform.TryGetComponent(out ISelectable selectable);

            // Highlight
            if (tryGetSelectable && !selectable.IsEqual(Game.Highlighted))
            {
                Game.Highlighted = selectable;
            }
        }
    }
}
