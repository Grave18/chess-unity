using System;
using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.ChessBoard.Pieces;
using Chess3D.Runtime.Core.Highlighting;
using Chess3D.Runtime.Core.InputManagement;
using Chess3D.Runtime.Core.MainCamera;
using Chess3D.Runtime.Core.UI.UnityGUI.Promotion;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.Logic.Players
{
    [UnityEngine.Scripting.Preserve]
    public class InputHuman : IInputHandler
    {
        // TODO: Move LayerMask to SO
        private const float MaxDistance = 100;
        private readonly LayerMask _layerMask = LayerMask.GetMask("Piece", "Square");

        private readonly Game _game;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly Camera _mainCamera;
        private readonly Highlighter _highlighter;
        private readonly ISettingsContainer settingsContainer;

        private bool _isPlaying;

        public InputHuman(Game game, IGameStateMachine gameStateMachine,
            ISettingsContainer settingsContainer, [Key(CameraKeys.Main)] Camera mainCamera,
            Highlighter highlighter)
        {
            _game = game;
            _highlighter = highlighter;
            _mainCamera = mainCamera;
            this.settingsContainer = settingsContainer;
            _gameStateMachine = gameStateMachine;
        }

        public UniTask Load()
        {
            return UniTask.CompletedTask;
        }

        public void StartInput()
        {
            _isPlaying = true;
        }

        public void UpdateInput()
        {
            if (!_isPlaying)
            {
                return;
            }

            // Cast ray from cursor
            Vector3 mousePos = InputController.MousePosition();
            Ray ray = _mainCamera.ScreenPointToRay(mousePos);
            Physics.Raycast(ray, out RaycastHit hit, MaxDistance, _layerMask);
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
                if (settingsContainer.S.GameSettings.IsAutoPromoteToQueen)
                {
                    uci += "q";
                    _gameStateMachine.Move(uci);
                    _game.Deselect();
                    return;
                }

                _gameStateMachine.Pause();
                // TODO: refactor Promotion
                // promotionView.RequestPromotedPiece(_game.CurrentTurnColor, pieceLetter =>
                // {
                //     _gameStateMachine.Play();
                //     uci += pieceLetter;
                //     _gameStateMachine.Move(uci);
                //     _game.Deselect();
                // });
            }
            else
            {
                _gameStateMachine.Move(uci);
                _game.Deselect();
            }
        }

        private static bool CanPromote(Piece piece, Square toSquare)
        {
            return piece is Pawn
                   && toSquare?.Rank is "1" or "8"
                   && (piece.CanMoveTo(toSquare, out _) || piece.CanCaptureAt(toSquare, out _));
        }

        public void StopInput()
        {
            _isPlaying = false;
        }
    }
}