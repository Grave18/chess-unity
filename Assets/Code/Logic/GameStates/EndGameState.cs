using Logic.MovesBuffer;

namespace Logic.GameStates
{
    public class EndGameState : GameState
    {
        protected Game Game { get; private set; }
        public override string Name => "End Game";

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

        public override void Move(string uci)
        {
            // Empty
        }

        public override void Undo()
        {
            if (Game.UciBuffer.CanUndo(out MoveData moveData))
            {
                Game.GameStateMachine.SetState(new UndoState(Game, moveData), isSetPreviousState:false);
            }
        }

        public override void Redo()
        {
            // Empty
        }

        public override void Play()
        {
            // Empty
        }

        public override void Pause()
        {
            // Empty
        }

        public override void Update()
        {
            // Empty
        }
    }
}