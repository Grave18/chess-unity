using Chess3D.Runtime.Core.Logic.GameStates;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Chess3D.Runtime.Core.Logic.Players
{
    [UnityEngine.Scripting.Preserve]
    public sealed class GameStateMachineOffline: IGameStateMachine
    {
        public IGameState State { get; private set; }

        public string StateName => State?.Name ?? "No State";

        private readonly CompositeDisposable _disposables = new();

        private GameStateMachineOffline()
        {
            Observable.EveryGameObjectUpdate().Subscribe(Tick).AddTo(_disposables);
        }

        public UniTask Load(IGameState warmUpState)
        {
            SetState(warmUpState);

            return UniTask.CompletedTask;
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

        private void Tick(long _)
        {
            State?.StateUpdate();
        }
    }
}