using Ui.InGame.ViewModels;
using UnityEngine;
using UtilsCommon.Singleton;

namespace ChessGame.Logic.MenuStates
{
    public class MenuStateMachine : SingletonBehaviour<MenuStateMachine>
    {
        [SerializeField] private GameObject gameCanvas;
        [SerializeField] private InGameMenuViewModel inGameMenuViewModel;

        private MenuState _currentState;

        private void Start()
        {
            ChangeState<GameState>();
        }

        public void ChangeState<T>() where T : MenuState
        {
            _currentState?.OnExit();

            if (typeof(T) == typeof(GameState))
            {
                _currentState = new GameState(this);
            }
            else if (typeof(T) == typeof(PauseState))
            {
                _currentState = new PauseState(this, inGameMenuViewModel, gameCanvas);
            }
            else if (typeof(T) == typeof(PopupState))
            {
                _currentState = new PopupState(this, gameCanvas);
            }
            else
            {
                _currentState = null;
            }

            _currentState?.OnEnter();
        }

        public void OpenPause()
        {
            _currentState?.OpenPause();
        }

        public void ClosePause()
        {
            _currentState?.ClosePause();
        }

        public void OpenPopup()
        {
            _currentState?.OpenPopup();
        }

        public void ClosePopup()
        {
            _currentState?.ClosePopupToGame();
        }

        public void ClosePopupToPause()
        {
            _currentState?.ClosePopupToPause();
        }

        public void ClosePopupToGame()
        {
            _currentState?.ClosePopupToGame();
        }
    }
}