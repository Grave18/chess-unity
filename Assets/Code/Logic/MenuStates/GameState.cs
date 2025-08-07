using UnityEngine;

namespace Logic.MenuStates
{
    public class GameState : MenuState
    {
        private readonly GameObject _gameCanvas;

        public GameState(MenuStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void OpenPause()
        {
            Machine.ChangeState<PauseState>();
        }

        public override void OpenPopup()
        {
            Machine.ChangeState<PopupState>();
        }
    }
}