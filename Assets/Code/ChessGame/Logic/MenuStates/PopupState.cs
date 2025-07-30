using UnityEngine;

namespace ChessGame.Logic.MenuStates
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
            Machine.ChangeState<GameState>();
        }

        public override void ClosePopupToPause()
        {
            Machine.ChangeState<PauseState>();
        }

        private void EnableProtection(bool value)
        {
            GInput.IsEnabled = !value;
            _gameCanvas.SetActive(!value);
        }
    }
}