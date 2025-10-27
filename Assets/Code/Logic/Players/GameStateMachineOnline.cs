using Logic.GameStates;
using PurrNet;
using PurrNet.StateMachine;
using TNRD;
using UnityEngine;

namespace Logic.Players
{
    public class GameStateMachineOnline : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private StateMachine stateMachine;

        [Header("States")]
        [SerializeField] private SerializableInterface<IGameState> warmupState;

        public string StateName => State?.Name ?? "No State";
        private IGameState State => stateMachine.currentStateNode as IGameState;

        public void Init(Game game)
        {
            _game = game;
        }

        public void SetState(IGameState state)
        {
            if (!isServer)
            {
                return;
            }

            stateMachine.SetState(state as StateNode);
        }

        public void SetState<T>(IGameState state, T data)
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

        public void Undo()
        {
            State?.Undo();
        }

        public void Redo()
        {
            State?.Redo();
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

        // TODO: remove or refactor
        private Game _game;
        // private IState _previousState;

        // public void SetPreviousState()
        // {
        //     if (_previousState != null)
        //     {
        //         SetState(_previousState);
        //         _previousState = null;
        //     }
        //     else
        //     {
        //         SetState(new IdleState(_game));
        //         Debug.Log("Go to default Idle state");
        //     }
        // }

        // private void Update()
        // {
        //     _state?.StateUpdate();
        // }
    }
}