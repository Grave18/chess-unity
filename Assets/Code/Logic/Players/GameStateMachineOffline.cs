using Logic.GameStates;
using TNRD;
using UnityEngine;

namespace Logic.Players
{
    public class GameStateMachineOffline : MonoBehaviour, IGameStateMachine
    {
        [Header("States")]
        [SerializeField] private SerializableInterface<IGameState> warmupState;

        public IGameState State { get; private set; }
        public string StateName => State?.Name ?? "No State";

        public void Init()
        {
            SetState(warmupState.Value);
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

        public void ResetState()
        {
            if (State is not WarmUpState)
            {
                SetState(warmupState.Value);
            }
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

        private void Update()
        {
            State?.StateUpdate();
        }
    }
}