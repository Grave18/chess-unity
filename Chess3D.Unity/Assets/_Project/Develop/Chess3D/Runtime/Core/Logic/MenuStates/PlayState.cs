using Chess3D.Runtime.Core.InputManagement;
using Chess3D.Runtime.Core.UI.UnityGUI.LeftPanel;

namespace Chess3D.Runtime.Core.Logic.MenuStates
{
    public sealed class PlayState : MenuState
    {
        private readonly GameCanvasEnabler gameCanvasEnabler;

        public PlayState(MenuStateMachine stateMachine, GameCanvasEnabler gameCanvasEnabler) : base(stateMachine)
        {
            this.gameCanvasEnabler = gameCanvasEnabler;
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