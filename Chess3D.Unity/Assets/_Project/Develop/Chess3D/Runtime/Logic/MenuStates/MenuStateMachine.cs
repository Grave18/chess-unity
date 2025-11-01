using Chess3D.Runtime.UtilsCommon.Singleton;
using Ui.InGame.ViewModels;
using UnityEngine;

namespace Chess3D.Runtime.Logic.MenuStates
{
    public class MenuStateMachine : SingletonBehaviour<MenuStateMachine>
    {
        [SerializeField] private GameObject gameCanvas;
        [SerializeField] private InGameMenuViewModel inGameMenuViewModel;

        private MenuState _currentState;

        public void ResetState()
        {
            SetState<PlayState>();
        }

        public void SetState<T>() where T : MenuState
        {
            _currentState?.OnExit();

            if (typeof(T) == typeof(MenuState))
            {
                throw new System.Exception($"{nameof(T)} is not a valid state");
            }
            if (typeof(T) == typeof(PlayState))
            {
                _currentState = new PlayState(this, gameCanvas);
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