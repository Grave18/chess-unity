using InputManagement;
using UnityEngine;

namespace Logic.MenuStates
{
    public class PopupState : MenuState
    {
        private readonly GameObject _gameCanvas;

        public PopupState(MenuStateMachine stateMachine, GameObject gameCanvas) : base(stateMachine)
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

        private void EnableProtection(bool value)
        {
            InputController.IsEnabled = !value;
            _gameCanvas.SetActive(!value);
        }
    }
}