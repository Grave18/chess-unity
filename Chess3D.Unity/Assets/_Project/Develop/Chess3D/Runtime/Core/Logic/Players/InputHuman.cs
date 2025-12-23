using System;
using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.ChessBoard.Pieces;
using Chess3D.Runtime.Core.MainCamera;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Core.Logic.Players
{
    [Preserve]
    public sealed class InputHuman : IInputHandler
    {
        private readonly Game _game;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISettingsContainer _settingsContainer;
        private readonly CoreEvents _coreEvents;
        private readonly CameraController _cameraController;

        private bool _isPlaying;

        public InputHuman(Game game, IGameStateMachine gameStateMachine,
            ISettingsContainer settingsContainer, CoreEvents coreEvents, CameraController cameraController)
        {
            _game = game;
            _coreEvents = coreEvents;
            _cameraController = cameraController;
            _settingsContainer = settingsContainer;
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
            if(_cameraController.IsHitSomething(out ISelectable selectable))
            {
                Click(selectable);
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
                if (_settingsContainer.S.GameSettings.IsAutoPromoteToQueen)
                {
                    uci += "q";
                    _gameStateMachine.Move(uci);
                    _game.Deselect();
                    return;
                }

                _gameStateMachine.Pause();

                _coreEvents.RequestPromotion.OnNext(letter =>
                {
                    _gameStateMachine.Play();
                    _gameStateMachine.Move(uci+letter);
                    _game.Deselect();
                });

            }
            else
            {
                _gameStateMachine.Move(uci);
                _game.Deselect();
            }
        }

        // Get uci with appended letter from presenter

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