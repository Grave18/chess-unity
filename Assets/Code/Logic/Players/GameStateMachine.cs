using Logic.GameStates;
using PurrNet;
using PurrNet.StateMachine;
using UnityEngine;

namespace Logic.Players
{
    public class GameStateMachine : NetworkBehaviour
    {
        [SerializeField] private StateMachine stateMachine;
        [SerializeField] private StateNode warmupState;

        public string StateName => State?.Name ?? "No State";
        private IState State => stateMachine.currentStateNode as IState;

        public void Init(Game game)
        {
            _game = game;
        }

        public void SetState(StateNode state)
        {
            if (!isServer)
            {
                return;
            }

            stateMachine.SetState(state);
        }

        public void SetState<T>(StateNode<T> state, T data)
        {
            if (!isServer)
            {
                return;
            }

            stateMachine.SetState(state, data);
        }

        [ServerRpc(requireOwnership: false)]
        public void Reset()
        {
            if (State is not WarmUpState)
            {
                SetState(warmupState);
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