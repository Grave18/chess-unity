namespace ChessGame.Logic.MenuStates
{
    public class MenuState
    {
        protected MenuStateMachine Machine { get; }

        protected MenuState(MenuStateMachine stateMachine)
        {
            Machine = stateMachine;
        }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void OpenPause() { }
        public virtual void ClosePause() { }
        public virtual void OpenPopup() { }
        public virtual void ClosePopupToGame() { }
        public virtual void ClosePopupToPause() { }
    }
}