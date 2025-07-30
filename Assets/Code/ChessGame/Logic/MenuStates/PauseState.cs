using Ui.InGame.ViewModels;
using UnityEngine;

namespace ChessGame.Logic.MenuStates
{
    public class PauseState : MenuState
    {
        private readonly InGameMenuViewModel _inGameMenuViewModel;
        private readonly GameObject _gameCanvas;

        public PauseState(MenuStateMachine stateMachine, InGameMenuViewModel inGameMenuViewModel, GameObject gameCanvas) : base(stateMachine)
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
            Machine.ChangeState<GameState>();
        }

        public override void OpenPopup()
        {
            Machine.ChangeState<PopupState>();
        }

        private void EnableProtection(bool value)
        {
            GInput.IsEnabled = !value;
            _gameCanvas.SetActive(!value);
        }
    }
}