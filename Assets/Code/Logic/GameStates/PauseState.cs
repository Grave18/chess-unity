using Logic.MovesBuffer;
using PurrNet.StateMachine;

namespace Logic.GameStates
{
    public class PauseState : StateNode,IState
    {
        public string Name => "Pause";

        protected Game Game { get; private set; }

        public PauseState(Game game)
        {
            Game = game;
        }

        public override void Enter()
        {
            Game.FirePause();
        }

        public override void Exit()
        {
            Game.FirePlay();
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
            if (Game.UciBuffer.CanUndo(out MoveData moveData))
            {
                // TODO: Game.GameStateMachine.SetState(new UndoState(Game, moveData));
            }
        }

        public void Redo()
        {
            if (Game.UciBuffer.CanRedo(out MoveData moveData))
            {
                // TODO: Game.GameStateMachine.SetState(new RedoState(Game, moveData));
            }
        }

        public void Play()
        {
            Game.GameStateMachine.SetState(new IdleState(Game));
        }

        public void Pause()
        {
            // Already paused
        }
    }
}