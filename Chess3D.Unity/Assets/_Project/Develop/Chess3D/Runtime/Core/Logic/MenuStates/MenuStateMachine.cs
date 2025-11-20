using Cysharp.Threading.Tasks;
using Chess3D.Runtime.Core.Ui;
using Chess3D.Runtime.Core.Ui.ViewModels;
using Chess3D.Runtime.Utilities;
using VContainer;

namespace Chess3D.Runtime.Core.Logic.MenuStates
{
    [UnityEngine.Scripting.Preserve]
    public sealed class MenuStateMachine: ILoadUnit
    {
        private readonly IObjectResolver _resolver;

        private MenuState _currentState;

        public MenuStateMachine(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public UniTask Load()
        {
            SetState<PlayState>();

            return UniTask.CompletedTask;
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
                var gameCanvas = _resolver.Resolve<GameCanvas>();
                _currentState = new PlayState(this, gameCanvas);
            }
            else if (typeof(T) == typeof(PauseState))
            {
                var inGameMenuViewModel = _resolver.Resolve<InGameMenuViewModel>();
                var gameCanvas = _resolver.Resolve<GameCanvas>();
                _currentState = new PauseState(this, inGameMenuViewModel, gameCanvas);
            }
            else if (typeof(T) == typeof(PopupState))
            {
                var gameCanvas = _resolver.Resolve<GameCanvas>();
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