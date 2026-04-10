using System;
using Chess3D.Runtime.Core.Logic.GameStates;
using Cysharp.Threading.Tasks;
using UniRx;
using VContainer;

namespace Chess3D.Runtime.Core.Logic.Players
{
    [UnityEngine.Scripting.Preserve]
    public sealed class GameStateMachineOffline: IGameStateMachine, IDisposable
    {
        public IGameState State { get; private set; }

        public string StateName => State?.Name ?? "No State";

        private readonly IObjectResolver _resolver;

        private readonly CompositeDisposable _disposables = new();

        private GameStateMachineOffline(IObjectResolver resolver)
        {
            _resolver = resolver;
            Observable.EveryGameObjectUpdate().Subscribe(Tick).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public UniTask Load(IGameState warmUpState)
        {
            SetState<WarmUpState>();

            return UniTask.CompletedTask;
        }

        public void SetState(IGameState state)
        {
            State?.Exit();
            State = state;
            State?.Enter();
        }

        public void SetState<TArg>(IGameState<TArg> state, TArg data)
        {
            State?.Exit();
            State = state;
            (State as IGameState<TArg>)?.Enter(data);
        }

        public void SetState<TState>() where TState : IGameState
        {
            State?.Exit();
            State = _resolver.Resolve<TState>();
            State?.Enter();
        }

        public void SetState<TState, TArg>(TArg data) where TState : IGameState<TArg>
        {
            State?.Exit();
            State = _resolver.Resolve<TState>();
            (State as IGameState<TArg>)?.Enter(data);
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