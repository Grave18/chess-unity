using Chess3D.Runtime.Core.Logic.MovesBuffer;
using PurrNet.StateMachine;
using TNRD;
using UnityEngine;

namespace Chess3D.Runtime.Core.Logic.GameStates
{
    public class PauseState : StateNode,IGameState
    {
        [Header("References")]
        [SerializeField] private Game game;

        [Header("States")]
        [SerializeField] private SerializableInterface<IGameState> idleState;
        [SerializeField] private SerializableInterface<IGameState<MoveData>> undoState;
        [SerializeField] private SerializableInterface<IGameState<MoveData>> redoState;

        public string Name => "Pause";

        public override void Enter()
        {
            Debug.Log("State: " + Name);

            game.FirePause();
        }

        public override void Exit()
        {
            game.FirePlay();
        }

        public void Undo()
        {
            if (game.UciBuffer.CanUndo(out MoveData moveData))
            {
                moveData.PreviousState = this;
                game.GameStateMachine.SetState(undoState.Value, moveData);
            }
        }

        public void Redo()
        {
            if (game.UciBuffer.CanRedo(out MoveData moveData))
            {
                moveData.PreviousState = this;
                game.GameStateMachine.SetState(redoState.Value, moveData);
            }
        }

        public override void StateUpdate()
        {
            // Nothing to update
        }

        public void Move(string uci)
        {
            // Can't move from Pause
        }

        public void Play()
        {
            game.GameStateMachine.SetState(idleState.Value);
        }

        public void Pause()
        {
            // Already paused
        }
    }
}