using Chess3D.Runtime.Core.InputManagement;
using Chess3D.Runtime.Core.UI.UnityGUI.LeftPanel;

namespace Chess3D.Runtime.Core.Logic.MenuStates
{
    public sealed class PopupState : MenuState
    {
        private readonly GameCanvasEnabler gameCanvasEnabler;

        public PopupState(MenuStateMachine stateMachine, GameCanvasEnabler gameCanvasEnabler) : base(stateMachine)
        {
            this.gameCanvasEnabler = gameCanvasEnabler;
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
                gameCanvasEnabler.Hide();
            }
            else
            {
                InputController.IsEnabled = true;
                gameCanvasEnabler.Show();
            }
        }
    }
}