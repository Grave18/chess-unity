using Logic.GameStates;

namespace Logic.Players
{
    public interface IGameStateMachine
    {
        string StateName { get; }
        void SetState(IGameState state);
        void SetState<T>(IGameState<T> state, T data);
        void ResetState();
        void Move(string uci);
        void Undo();
        void Redo();
        void Play();
        void Pause();
    }
}