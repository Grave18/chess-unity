namespace Chess3D.Runtime.Core.Logic.MenuStates
{
    public abstract class MenuState
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