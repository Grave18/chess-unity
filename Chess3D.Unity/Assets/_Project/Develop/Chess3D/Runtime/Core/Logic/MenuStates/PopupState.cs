using Chess3D.Runtime.Core.InputManagement;
using Chess3D.Runtime.Core.Logic.Players;
using Chess3D.Runtime.Core.UI.UnityGUI.LeftPanel;

namespace Chess3D.Runtime.Core.Logic.MenuStates
{
    public sealed class PopupState : MenuState
    {
        private readonly GameCanvasEnabler _gameCanvasEnabler;
        private readonly IGameStateMachine _gameStateMachine;

        public PopupState(MenuStateMachine stateMachine, GameCanvasEnabler gameCanvasEnabler, IGameStateMachine gameStateMachine) : base(stateMachine)
        {
            _gameCanvasEnabler = gameCanvasEnabler;
            _gameStateMachine = gameStateMachine;
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
                _gameStateMachine.Pause();
                InputController.IsEnabled = false;
                _gameCanvasEnabler.Hide();
            }
            else
            {
                _gameStateMachine.Play();
                InputController.IsEnabled = true;
                _gameCanvasEnabler.Show();
            }
        }
    }
}