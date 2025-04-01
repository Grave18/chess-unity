using Logic.MovesBuffer;

namespace Logic.Players.GameStates
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
            if(IsGameOver())
            {
                Game.FireEnd();
            }
            else
            {
                _isRunning = true;
                Game.Competitors.StartPlayer();
            }
        }

        private bool IsGameOver()
        {
            return Game.CheckType is CheckType.CheckMate or CheckType.Stalemate || IsTimeOut();
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
            if (!_isRunning)
            {
                return;
            }

            if (IsTimeOut())
            {
                _isRunning = false;
            }

            Game.Competitors.UpdatePlayer();
        }

        private bool IsTimeOut()
        {
            return Game.CheckType is CheckType.TimeOutWhite or CheckType.TimeOutBlack;
        }
    }
}