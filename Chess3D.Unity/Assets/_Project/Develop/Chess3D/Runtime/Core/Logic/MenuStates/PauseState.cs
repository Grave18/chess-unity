using Chess3D.Runtime.Core.InputManagement;
using Chess3D.Runtime.Core.UI;
using Chess3D.Runtime.Core.UI.ViewModels;

namespace Chess3D.Runtime.Core.Logic.MenuStates
{
    public class PauseState : MenuState
    {
        private readonly CoreMainViewModel coreMainViewModel;
        private readonly GameCanvas _gameCanvas;

        public PauseState(MenuStateMachine stateMachine, CoreMainViewModel coreMainViewModel, GameCanvas gameCanvas) : base(stateMachine)
        {
            _gameCanvas = gameCanvas;
            this.coreMainViewModel = coreMainViewModel;
        }

        public override void OnEnter()
        {
            coreMainViewModel.IsOpened = true;
            EnableProtection(true);
        }

        public override void OnExit()
        {
            coreMainViewModel.IsOpened = false;
            EnableProtection(false);
        }

        public override void ClosePause()
        {
            Machine.SetState<PlayState>();
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