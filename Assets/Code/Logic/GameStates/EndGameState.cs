using Logic.MovesBuffer;
using PurrNet.StateMachine;

namespace Logic.GameStates
{
    public class EndGameState : StateNode, IState
    {
        protected Game Game { get; private set; }
        public string Name => "End Game";

        public EndGameState(Game game)
        {
            Game = game;
        }

        public override void Enter()
        {
            Game.FireEnd();
        }

        public override void Exit()
        {
            // Empty
        }

        public override void StateUpdate()
        {
            // Empty
        }

        public void Move(string uci)
        {
            // Empty
        }

        public void Undo()
        {
            if (Game.UciBuffer.CanUndo(out MoveData moveData))
            {
                // TODO: Game.GameStateMachine.SetState(new UndoState(Game, moveData), isSetPreviousState:false);
            }
        }

        public void Redo()
        {
            // Empty
        }

        public void Play()
        {
            // Empty
        }

        public void Pause()
        {
            // Empty
        }
    }
}