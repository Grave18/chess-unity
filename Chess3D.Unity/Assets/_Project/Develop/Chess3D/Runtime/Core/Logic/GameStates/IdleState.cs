using Chess3D.Runtime.Core.Logic.MovesBuffer;
using Chess3D.Runtime.Core.Logic.Players;
using UniRx;
using VContainer;

namespace Chess3D.Runtime.Core.Logic.GameStates
{
    public sealed class IdleState : IGameState
    {
        private Game _game;
        private Competitors _competitors;
        private UciBuffer _uciBuffer;
        private CoreEvents _coreEvents;
        private IGameStateMachine _gameStateMachine;

        private bool _isRunning;

        public string Name => "Idle";

        [Inject]
        public void Construct(Game game, Competitors competitors, UciBuffer uciBuffer, CoreEvents coreEvents,
            IGameStateMachine gameStateMachine)
        {
            _game = game;
            _competitors = competitors;
            _uciBuffer = uciBuffer;
            _coreEvents = coreEvents;
            _gameStateMachine = gameStateMachine;
        }

        public async void Enter()
        {
            if (_game.IsEndGame)
            {
                EndGame();
                return;
            }

            _isRunning = true;
            await _competitors.StartPlayer();

            _coreEvents.OnIdle.OnNext(Unit.Default);
        }

        public void Exit()
        {
            _isRunning = false;
            _competitors.StopPlayer();
        }

        public void Move(string uci)
        {
            _gameStateMachine.SetState<MoveState, string>(uci);
        }

        public void Undo()
        {
            if (_uciBuffer.CanUndo(out MoveData moveData))
            {
                moveData.PreviousState = this;
                _gameStateMachine.SetState<UndoState, MoveData>(moveData);
            }
        }

        public void Redo()
        {
            if (_uciBuffer.CanRedo(out MoveData moveData))
            {
                moveData.PreviousState = this;
                _gameStateMachine.SetState<RedoState, MoveData>(moveData);
            }
        }

        public void Pause()
        {
            _gameStateMachine.SetState<PauseState>();
        }

        public void StateUpdate()
        {
            if (!_isRunning)
            {
                return;
            }

            if (_game.IsEndGame)
            {
                EndGame();
            }

            _competitors.UpdatePlayer();
        }

        private void EndGame()
        {
            _gameStateMachine.SetState<EndGameState>();
            _competitors.StopPlayer();
        }

        public void Play()
        {
            // Already playing
        }
    }
}