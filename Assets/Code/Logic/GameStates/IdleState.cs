using Logic.MovesBuffer;

namespace Logic.GameStates
{
    public class IdleState : GameState
    {
        private bool _isRunning;

        public IdleState(Game game) : base(game)
        {
        }

        public override string Name => "Idle";

        public override void Enter()
        {
            if (Game.IsEndGame)
            {
                EndGame();
                return;
            }

            _isRunning = true;
            Game.Competitors.StartPlayer();
        }

        public override void Exit()
        {
            Game.Competitors.StopPlayer();
        }

        public override void Move(string uci)
        {
            Game.GameStateMachine.SetState(new MoveState(Game, uci));
        }

        public override void Undo()
        {
            if (Game.UciBuffer.CanUndo(out MoveData moveData))
            {
                Game.GameStateMachine.SetState(new UndoState(Game, moveData));
            }
        }

        public override void Redo()
        {
            if (Game.UciBuffer.CanRedo(out MoveData moveData))
            {
                Game.GameStateMachine.SetState(new RedoState(Game, moveData));
            }
        }

        public override void Play()
        {
            // Already playing
        }

        public override void Pause()
        {
            Game.GameStateMachine.SetState(new PauseState(Game));
        }

        public override void Update()
        {
            if (!_isRunning)
            {
                return;
            }

            if (Game.IsEndGame)
            {
                EndGame();
                return;
            }

            Game.Competitors.UpdatePlayer();
        }

        private void EndGame()
        {
            Game.GameStateMachine.SetState(new EndGameState(Game));
            Game.Competitors.StopPlayer();
        }
    }
}