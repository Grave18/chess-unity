using Logic.GameStates;
using TNRD;
using UnityEngine;

namespace Logic.Players
{
    public class GameStateMachineOffline : MonoBehaviour, IGameStateMachine
    {
        [Header("States")]
        [SerializeField] private SerializableInterface<IGameState> warmupState;

        private IGameState _state;

        public string StateName => _state?.Name ?? "No State";

        public void Init(Game game)
        {
            _game = game;
            SetState(warmupState.Value);
        }

        public void SetState(IGameState state)
        {
            _state?.Exit();
            _state = state;
            state?.Enter();
        }

        public void SetState<T>(IGameState<T> state, T data)
        {
            _state?.Exit();
            _state = state;
            state?.Enter(data);
        }

        public void ResetState()
        {
            if (_state is not WarmUpState)
            {
                SetState(warmupState.Value);
            }
        }

        public void Move(string uci)
        {
            _state?.Move(uci);
        }

        public void Undo()
        {
            _state?.Undo();
        }

        public void Redo()
        {
            _state?.Redo();
        }

        public void Play()
        {
            _state?.Play();
        }

        public void Pause()
        {
            _state?.Pause();
        }

        private void Update()
        {
            _state?.StateUpdate();
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
    }
}