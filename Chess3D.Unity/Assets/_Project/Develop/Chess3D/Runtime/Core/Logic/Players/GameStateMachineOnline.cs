using Chess3D.Runtime.Core.Logic.GameStates;
using Cysharp.Threading.Tasks;
using PurrNet;
using PurrNet.StateMachine;
using UnityEngine;

namespace Chess3D.Runtime.Core.Logic.Players
{
    public sealed class GameStateMachineOnline : NetworkBehaviour, IGameStateMachine
    {
        [Header("References")]
        [SerializeField] private StateMachine stateMachine;

        public string StateName => State?.Name ?? "No State";
        public IGameState State => stateMachine.currentStateNode as IGameState;

        public async UniTask Load(IGameState warmupState)
        {
            SetState(warmupState);
            await UniTask.WaitUntil(() => State is not WarmUpState);
        }

        public void SetState(IGameState state)
        {
            if (!isServer)
            {
                return;
            }

            stateMachine.SetState(state as StateNode);
        }

        public void SetState<T>(IGameState<T> state, T data)
        {
            if (!isServer)
            {
                return;
            }

            stateMachine.SetState(state as StateNode<T>, data);
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