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
            _isRunning = true;
            Game.Competitors.StartPlayer();
        }

        public override void Exit(string nextState)
        {
            Game.Competitors.StopPlayer();
        }

        public override void Move(string uci)
        {
            Game.Machine.SetState(new MoveState(Game, uci));
        }

        public override void Undo()
        {
            if (Game.UciBuffer.CanUndo(out MoveData moveData))
            {
                Game.Machine.SetState(new UndoState(Game, moveData));
            }
        }

        public override void Redo()
        {
            if (Game.UciBuffer.CanRedo(out MoveData moveData))
            {
                Game.Machine.SetState(new RedoState(Game, moveData));
            }
        }

        public override void Play()
        {
            // Already playing
        }

        public override void Pause()
        {
            Game.Machine.SetState(new PauseState(Game));
        }

        public override void Update()
        {
            if (Game.IsCheckMate)
            {
                Game.Checkmate();
                return;
            }

            if (!_isRunning)
            {
                return;
            }

            Game.Competitors.UpdatePlayer();
        }
    }
}