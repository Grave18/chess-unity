using ChessGame.Logic.MovesBuffer;

namespace ChessGame.Logic.GameStates
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
            _isRunning = true;
            Game.Competitors.StartPlayer();
        }

        public override void Exit(string nextState)
        {
            Game.Competitors.StopPlayer();
        }

        public override void Move(string uci)
        {
            Game.SetState(new MoveState(Game, uci));
        }

        public override void Undo()
        {
            if (Game.UciBuffer.CanUndo(out MoveData moveData))
            {
                Game.SetState(new UndoState(Game, moveData));
            }
        }

        public override void Redo()
        {
            if (Game.UciBuffer.CanRedo(out MoveData moveData))
            {
                Game.SetState(new RedoState(Game, moveData));
            }
        }

        public override void Play()
        {
            // Already playing
        }

        public override void Pause()
        {
            Game.SetState(new PauseState(Game));
        }

        public override void Update()
        {
            if (Game.IsGameOver())
            {
                Game.SetState(new EndGameState(Game));
            }

            if (!_isRunning)
            {
                return;
            }

            Game.Competitors.UpdatePlayer();
        }
    }
}