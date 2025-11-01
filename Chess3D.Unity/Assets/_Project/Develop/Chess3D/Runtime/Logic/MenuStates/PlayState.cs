using Chess3D.Runtime.InputManagement;
using UnityEngine;

namespace Chess3D.Runtime.Logic.MenuStates
{
    public class PlayState : MenuState
    {
        private readonly GameObject _gameCanvas;

        public PlayState(MenuStateMachine stateMachine, GameObject gameCanvas) : base(stateMachine)
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

        private void EnableProtection(bool value)
        {
            InputController.IsEnabled = !value;
            _gameCanvas.SetActive(!value);
        }
    }
}