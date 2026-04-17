using Chess3D.Runtime.Core.Logic.GameStates;

namespace Chess3D.Runtime.Core.Logic.Players
{
    public interface IGameStateMachine
    {
        IGameState State { get; }
        string StateName { get; }
        public void SetState(IGameState state);
        public void SetState<TArg>(IGameState<TArg> state, TArg data);
        void SetState<TState>() where TState : IGameState;
        public void SetState<TState, TArg>(TArg data) where TState : IGameState<TArg>;
        void Move(string uci);
        void Undo();
        void Redo();
        void Play();
        void Pause();
    }
}