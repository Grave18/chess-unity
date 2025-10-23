using Logic.MovesBuffer;

namespace Logic.GameStates
{
    public class IdleState : GameState
    {
        private readonly Game _game;
        private bool _isRunning;

        public IdleState(Game game)
        {
            _game = game;
        }

        public override string Name => "Idle";

        public override void Enter()
        {
            if (_game.IsEndGame)
            {
                EndGame();
                return;
            }

            _isRunning = true;
            _game.Competitors.StartPlayer();
        }

        public override void Exit()
        {
            _game.Competitors.StopPlayer();
        }

        public override void Move(string uci)
        {
            _game.GameStateMachine.SetState(new MoveState(_game, uci));
        }

        public override void Undo()
        {
            if (_game.UciBuffer.CanUndo(out MoveData moveData))
            {
                _game.GameStateMachine.SetState(new UndoState(_game, moveData));
            }
        }

        public override void Redo()
        {
            if (_game.UciBuffer.CanRedo(out MoveData moveData))
            {
                _game.GameStateMachine.SetState(new RedoState(_game, moveData));
            }
        }

        public override void Play()
        {
            // Already playing
        }

        public override void Pause()
        {
            _game.GameStateMachine.SetState(new PauseState(_game));
        }

        public override void Update()
        {
            if (!_isRunning)
            {
                return;
            }

            if (_game.IsEndGame)
            {
                EndGame();
            }

            _game.Competitors.UpdatePlayer();
        }

        private void EndGame()
        {
            _game.GameStateMachine.SetState(new EndGameState(_game));
            _game.Competitors.StopPlayer();
        }
    }
}