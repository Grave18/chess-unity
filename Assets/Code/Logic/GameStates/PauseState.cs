using Logic.MovesBuffer;
using PurrNet.StateMachine;
using UnityEngine;

namespace Logic.GameStates
{
    public class PauseState : StateNode,IState
    {
        [Header("References")]
        [SerializeField] private Game game;

        [Header("States")]
        [SerializeField] private StateNode idleState;

        public string Name => "Pause";

        public override void Enter()
        {
            game.FirePause();
        }

        public override void Exit()
        {
            game.FirePlay();
        }

        public override void StateUpdate()
        {
            // Nothing to update
        }

        public void Move(string uci)
        {
            // Can't move from Pause
        }

        public void Undo()
        {
            if (game.UciBuffer.CanUndo(out MoveData moveData))
            {
                // TODO: Game.GameStateMachine.SetState(new UndoState(Game, moveData));
            }
        }

        public void Redo()
        {
            if (game.UciBuffer.CanRedo(out MoveData moveData))
            {
                // TODO: Game.GameStateMachine.SetState(new RedoState(Game, moveData));
            }
        }

        public void Play()
        {
            game.GameStateMachine.SetState(idleState);
        }

        public void Pause()
        {
            // Already paused
        }
    }
}