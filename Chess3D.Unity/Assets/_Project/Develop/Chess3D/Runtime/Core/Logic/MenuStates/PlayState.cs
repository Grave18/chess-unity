using Chess3D.Runtime.Core.InputManagement;
using Chess3D.Runtime.Core.Ui;
using UnityEngine;

namespace Chess3D.Runtime.Core.Logic.MenuStates
{
    public class PlayState : MenuState
    {
        private readonly GameCanvas _gameCanvas;

        public PlayState(MenuStateMachine stateMachine, GameCanvas gameCanvas) : base(stateMachine)
        {
            _gameCanvas = gameCanvas;
        }

        public override void OnEnter()
        {
            EnableProtection(false);
        }

        public override void OpenPause()
        {
            Machine.SetState<PauseState>();
        }

        public override void OpenPopup()
        {
            Machine.SetState<PopupState>();
        }

        private void EnableProtection(bool isProtectionEnabled)
        {
            if (isProtectionEnabled)
            {
                InputController.IsEnabled = false;
                _gameCanvas.Hide();
            }
            else
            {
                InputController.IsEnabled = true;
                _gameCanvas.Show();
            }
        }
    }
}