using Chess3D.Runtime.Core.Logic.GameStates;
using Cysharp.Threading.Tasks;
using PurrNet;
using PurrNet.StateMachine;
using VContainer;

namespace Chess3D.Runtime.Core.Logic.Players
{
    public sealed class GameStateMachineOnline : NetworkBehaviour, IGameStateMachine
    {
        private StateMachine _stateMachine;

        public string StateName => State?.Name ?? "No State";
        public IGameState State => _stateMachine.currentStateNode as IGameState;

        [Inject]
        public void Construct(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public UniTask Load(IGameState warmupState)
        {
            SetState(warmupState);

            return UniTask.CompletedTask;
        }

        public void SetState(IGameState state)
        {
            if (!isServer)
            {
                return;
            }

            _stateMachine.SetState(state as StateNode);
        }

        public void SetState<T>(IGameState<T> state, T data)
        {
            if (!isServer)
            {
                return;
            }

            _stateMachine.SetState(state as StateNode<T>, data);
        }

        [ServerRpc(requireOwnership: false)]
        public void Move(string uci)
        {
            State?.Move(uci);
        }

        [ServerRpc(requireOwnership: false)]
        public void Play()
        {
            State?.Play();
        }

        [ServerRpc(requireOwnership: false)]
        public void Pause()
        {
            State?.Pause();
        }

        // Unused
        public void Undo() { }
        public void Redo() { }
    }
}