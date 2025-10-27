using Logic.MovesBuffer;
using PurrNet.StateMachine;
using TNRD;
using UnityEngine;

namespace Logic.GameStates
{
    public class IdleState : StateNode, IGameState
    {
        [Header("References")]
        [SerializeField] private Game game;

        [Header("States")]
        [SerializeField] private SerializableInterface<IGameState> endGameState;
        [SerializeField] private SerializableInterface<IGameState> pauseState;
        [SerializeField] private SerializableInterface<IGameState<string>> moveState;

        private bool _isRunning;

        public string Name => "Idle";

        public override void Enter()
        {
            Debug.Log("State: " + Name);

            if (game.IsEndGame)
            {
                EndGame();
                return;
            }

            _isRunning = true;
            game.Competitors.StartPlayer();

            game.FireIdle();
        }

        public override void Exit()
        {
            _isRunning = false;
            game.Competitors.StopPlayer();
        }

        public void Move(string uci)
        {
            game.GameStateMachine.SetState(moveState.Value, uci);
        }

        public void Undo()
        {
            if (game.UciBuffer.CanUndo(out MoveData moveData))
            {
                // TODO _game.GameStateMachine.SetState(new UndoState(_game, moveData));
            }
        }

        public void Redo()
        {
            if (game.UciBuffer.CanRedo(out MoveData moveData))
            {
                // TODO _game.GameStateMachine.SetState(new RedoState(_game, moveData));
            }
        }

        public void Play()
        {
            // Already playing
        }

        public void Pause()
        {
            game.GameStateMachine.SetState(pauseState.Value);
        }

        public override void StateUpdate()
        {
            if (!_isRunning)
            {
                return;
            }

            if (game.IsEndGame)
            {
                EndGame();
            }

            game.Competitors.UpdatePlayer();
        }

        private void EndGame()
        {
            game.GameStateMachine.SetState(endGameState.Value);
            game.Competitors.StopPlayer();
        }
    }
}