using System;
using ChessBoard;
using ChessBoard.Pieces;
using Highlighting;
using InputManagement;
using UnityEngine;
using UnityUi.InGame.Promotion;

namespace Logic.Players
{
    public class PlayerOffline : IPlayer
    {
        private readonly Game _game;
        private readonly Camera _mainCamera;
        private readonly PromotionPanel _promotionPanel;
        private readonly Highlighter _highlighter;
        private readonly LayerMask _layerMask;
        private readonly bool _isAutoPromoteToQueen;

        private const float MaxDistance = 100;

        public PlayerOffline(Game game, Camera mainCamera, Highlighter highlighter, LayerMask layerMask,
            bool isAutoPromoteToQueen, PromotionPanel promotionPanel)
        {
            _game = game;
            _highlighter = highlighter;
            _mainCamera = mainCamera;
            _layerMask = layerMask;
            _isAutoPromoteToQueen = isAutoPromoteToQueen;
            _promotionPanel = promotionPanel;
        }

        public void StartPlayer()
        {
            // Empty
        }

        public void UpdatePlayer()
        {
            // Cast ray from cursor
            Vector3 mousePos = InputController.MousePosition();
            Ray ray = _mainCamera.ScreenPointToRay(mousePos);
            bool isHit = Physics.Raycast(ray, out RaycastHit hit, MaxDistance, _layerMask);
            Transform hitTransform = hit.transform;

            if (InputController.Lmb())
            {
                ISelectable selectable = null;

                if (hitTransform != null)
                {
                    hitTransform.TryGetComponent(out selectable);
                }

                Click(selectable);
                _highlighter.UpdateHighlighting();
            }
        }

        private void Click(ISelectable selectable)
        {
            // Deselect if not hit anything
            if (selectable == null)
            {
                _game.Deselect();
                return;
            }

            // Select if clicked on current turn piece
            if (_game.CanSelect(selectable))
            {
                _game.Select(selectable);
                return;
            }

            // Exit early if no selected piece
            if (_game.Selected == null)
            {
                return;
            }

            ConstructUci(selectable);
        }

        /// Construct Uci string of move. example: "b1a1q", "e2e4"
        private void ConstructUci(ISelectable selectable)
        {
            Piece piece = _game.Selected.GetPiece();
            Square fromSquare = _game.Selected.GetSquare();
            Square toSquare = selectable.GetSquare();

            string uci = string.Empty;
            try
            {
                uci = $"{fromSquare.Address}{toSquare.Address}";
            }
            catch (NullReferenceException)
            {
                Debug.Log("Move to square is null");
            }

            if (CanPromote(piece, toSquare))
            {
                if (_isAutoPromoteToQueen)
                {
                    uci += "q";
                    Move(uci);
                    return;
                }

                _game.GameStateMachine.Pause();
                _promotionPanel.RequestPromotedPiece(_game.CurrentTurnColor, pieceLetter =>
                {
                    _game.GameStateMachine.Play();
                    uci += pieceLetter;
                    Move(uci);
                });
            }
            else
            {
                Move(uci);
            }
        }

        public void Move(string uci)
        {
            _game.GameStateMachine.Move(uci);
            _game.Deselect();
        }

        private static bool CanPromote(Piece piece, Square toSquare)
        {
            return piece is Pawn
                   && toSquare?.Rank is "1" or "8"
                   && (piece.CanMoveTo(toSquare, out _) || piece.CanCaptureAt(toSquare, out _));
        }

        public void StopPlayer()
        {
            // Empty
        }
    }
}