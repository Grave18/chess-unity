using Logic.GameStates;
using PurrNet;
using PurrNet.StateMachine;
using UnityEngine;

namespace Logic.Players
{
    public class GameStateMachine : NetworkBehaviour
    {
        [SerializeField] private StateMachine stateMachine;

        private IState State => stateMachine.currentStateNode as IState;
        public string StateName => State?.Name ?? "No State";

        public void Init(Game game)
        {
            _game = game;
        }

        public void SetState(StateNode state)
        {
            stateMachine.SetState(state);
        }

        public void SetState<T>(StateNode<T> state, T data)
        {
            stateMachine.SetState(state, data);
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