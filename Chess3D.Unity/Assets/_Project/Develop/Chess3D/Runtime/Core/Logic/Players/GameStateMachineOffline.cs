using Chess3D.Runtime.Core.Logic.GameStates;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Chess3D.Runtime.Core.Logic.Players
{
    [UnityEngine.Scripting.Preserve]
    public sealed class GameStateMachineOffline : MonoBehaviour, IGameStateMachine
    {
        [SerializeField] private WarmUpState warmUpState;
        [SerializeField] private GameStateMachineScope gameStateMachineScope;

        public IGameState State { get; private set; }

        public string StateName => State?.Name ?? "No State";

        public async UniTask Load()
        {
            gameStateMachineScope.Build();
            SetState(warmUpState);
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

        public void Update()
        {
            State?.StateUpdate();
        }
    }
}