using Chess3D.Runtime.Core.Logic.GameStates;

namespace Chess3D.Runtime.Core.Logic.Players
{
    public interface IGameStateMachine
    {
        IGameState State { get; }
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