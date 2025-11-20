using Chess3D.Runtime.Core.Logic.GameStates;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using VContainer.Unity;

namespace Chess3D.Runtime.Core.Logic.Players
{
    [Preserve]
    public sealed class GameStateMachineOffline : IGameStateMachine, ITickable
    {
        public IGameState State { get; private set; }

        public string StateName => State?.Name ?? "No State";

        public async UniTask Load(IGameState warmupState)
        {
            SetState(warmupState);
            await UniTask.WaitUntil(() => State is not WarmUpState);
        }

        public void SetState(IGameState state)
        {
            State?.Exit();
            State = state;
            state?.Enter();
        }

        public void SetState<T>(IGameState<T> state, T data)
        {
            State?.Exit();
            State = state;
            state?.Enter(data);
        }

        public void Move(string uci)
        {
            State?.Move(uci);
        }

        public void Undo()
        {
            State?.Undo();
        }

        public void Redo()
        {
            State?.Redo();
        }

        public void Play()
        {
            State?.Play();
        }

        public void Pause()
        {
            State?.Pause();
        }

        public void Tick()
        {
            State?.StateUpdate();
        }
    }
}