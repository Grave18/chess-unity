using Chess3D.Runtime.Core.InputManagement;
using Chess3D.Runtime.Core.Ui;
using InGameMenuViewModel = Chess3D.Runtime.Core.Ui.ViewModels.InGameMenuViewModel;

namespace Chess3D.Runtime.Core.Logic.MenuStates
{
    public class PauseState : MenuState
    {
        private readonly InGameMenuViewModel _inGameMenuViewModel;
        private readonly GameCanvas _gameCanvas;

        public PauseState(MenuStateMachine stateMachine, InGameMenuViewModel inGameMenuViewModel, GameCanvas gameCanvas) : base(stateMachine)
        {
            _gameCanvas = gameCanvas;
            _inGameMenuViewModel = inGameMenuViewModel;
        }

        public override void OnEnter()
        {
            _inGameMenuViewModel.IsOpened = true;
            EnableProtection(true);
        }

        public override void OnExit()
        {
            _inGameMenuViewModel.IsOpened = false;
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