using Chess3D.Runtime.Core.InputManagement;
using Chess3D.Runtime.Core.Ui;

namespace Chess3D.Runtime.Core.Logic.MenuStates
{
    public class PopupState : MenuState
    {
        private readonly GameCanvas _gameCanvas;

        public PopupState(MenuStateMachine stateMachine, GameCanvas gameCanvas) : base(stateMachine)
        {
            _gameCanvas = gameCanvas;
        }

        public override void OnEnter()
        {
            EnableProtection(true);
        }

        public override void OnExit()
        {
            EnableProtection(false);
        }

        public override void ClosePopupToGame()
        {
            Machine.SetState<PlayState>();
        }

        public override void ClosePopupToPause()
        {
            Machine.SetState<PauseState>();
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