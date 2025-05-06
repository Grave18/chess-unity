using System;
using ChessGame.ChessBoard;
using ChessGame.ChessBoard.Pieces;
using Highlighting;
using PurrNet;
using Ui.Game.Promotion;
using UnityEngine;

namespace ChessGame.Logic.Players
{
    public class PlayerOnline : NetworkBehaviour, IPlayer
    {
        private Game _game;
        private Camera _mainCamera;
        private PromotionPanel _promotionPanel;
        private PieceColor _color;
        private Highlighter _highlighter;
        private LayerMask _layerMask;
        private bool _isAutoPromoteToQueen;

        private const float MaxDistance = 100;

        public void Init(Game game, Camera mainCamera, Highlighter highlighter, LayerMask layerMask,
            bool isAutoPromoteToQueen, PromotionPanel promotionPanel, PieceColor color)
        {
            _game = game;
            _highlighter = highlighter;
            _mainCamera = mainCamera;
            _layerMask = layerMask;
            _isAutoPromoteToQueen = isAutoPromoteToQueen;
            _promotionPanel = promotionPanel;
            _color = color;
        }

        public void StartPlayer()
        {
            // Empty
        }

        public void UpdatePlayer()
        {
            if (isController)
            {
                Input();
            }
        }

        private void Input()
        {
            // Cast ray from cursor
            Vector3 mousePos = UnityEngine.Input.mousePosition;
            Ray ray = _mainCamera.ScreenPointToRay(mousePos);
            bool isHit = Physics.Raycast(ray, out RaycastHit hit, MaxDistance, _layerMask);
            Transform hitTransform = hit.transform;

            if (UnityEngine.Input.GetButtonDown("Fire1"))
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

                _game.Pause();
                _promotionPanel.RequestPromotedPiece(_game.CurrentTurnColor, pieceLetter =>
                {
                    _game.Play();
                    uci += pieceLetter;
                    Move(uci);
                });
            }
            else
            {
                Move(uci);
            }
        }

        [ObserversRpc]
        private void Move(string uci)
        {
            _game.Move(uci);
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