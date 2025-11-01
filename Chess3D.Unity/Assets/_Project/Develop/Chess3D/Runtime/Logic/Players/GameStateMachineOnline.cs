using Chess3D.Runtime.Logic.GameStates;
using PurrNet;
using PurrNet.StateMachine;
using TNRD;
using UnityEngine;

namespace Chess3D.Runtime.Logic.Players
{
    public class GameStateMachineOnline : NetworkBehaviour, IGameStateMachine
    {
        [Header("References")]
        [SerializeField] private StateMachine stateMachine;

        [Header("States")]
        [SerializeField] private SerializableInterface<IGameState> warmupState;

        public string StateName => State?.Name ?? "No State";
        public IGameState State => stateMachine.currentStateNode as IGameState;

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
        public void ResetState()
        {
            if (State is not WarmUpState)
            {
                SetState(warmupState.Value);
            }
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