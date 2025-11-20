using Chess3D.Runtime.Core.Logic.GameStates;
using Chess3D.Runtime.Utilities;

namespace Chess3D.Runtime.Core.Logic.Players
{
    public interface IGameStateMachine : ILoadUnit<IGameState>
    {
        IGameState State { get; }
        string StateName { get; }
        void SetState(IGameState state);
        void SetState<T>(IGameState<T> state, T data);
        void Move(string uci);
        void Undo();
        void Redo();
        void Play();
        void Pause();
    }
}