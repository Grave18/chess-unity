using Logic.GameStates;
using UnityEngine;

namespace Logic.Players
{
    public class GameStateMachine : MonoBehaviour
    {
        private Game _game;

        private GameState _state;
        private GameState _previousState;

        public string StateName => _state?.Name ?? "No State";

        public void SetState(GameState state, bool isSetPreviousState = true)
        {
            _previousState = isSetPreviousState
                ? _state
                : null;

            _state?.Exit();
            _state = state;
            _state?.Enter();
        }

        public void Init(Game game)
        {
            _game = game;
        }

        public void SetPreviousState()
        {
            if (_previousState != null)
            {
                SetState(_previousState);
                _previousState = null;
            }
            else
            {
                SetState(new IdleState(_game));
                Debug.Log("Go to default Idle state");
            }
        }

        private void Update()
        {
            _state?.Update();
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
    }
}