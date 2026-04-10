using System;
using Chess3D.Runtime.Core.Logic.MovesBuffer;
using Chess3D.Runtime.Core.Logic.Players;
using Chess3D.Runtime.Utilities;
using Chess3D.Runtime.Utilities.Logging;
using Cysharp.Threading.Tasks;
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
        private IObjectResolver _resolver;

        private bool _isRunning;
        private bool _isInitialized;

        public string Name => "Idle";

        [Inject]
        public void Construct(Game game, Competitors competitors, UciBuffer uciBuffer, CoreEvents coreEvents,
            IGameStateMachine gameStateMachine, IObjectResolver resolver)
        {
            _game = game;
            _competitors = competitors;
            _uciBuffer = uciBuffer;
            _coreEvents = coreEvents;
            _gameStateMachine = gameStateMachine;
            _resolver = resolver;

            _isInitialized = true;
        }

        public async void Enter()
        {
            try
            {
                await UniTask.WaitUntil(() => _isInitialized);

                if (_game.IsEndGame)
                {
                    EndGame();
                    return;
                }

                _isRunning = true;
                _competitors.StartPlayer();

                _coreEvents.OnIdle.OnNext(Unit.Default);
            }
            catch (Exception e)
            {
                Log.Default.E("GAMESTATE",e.Message);
            }
        }

        public void Exit()
        {
            _isRunning = false;
            _competitors.StopPlayer();
        }

        public void Move(string uci)
        {
            _gameStateMachine.SetState(_resolver.Resolve<MoveState>(), uci);
        }

        public void Undo()
        {
            if (_uciBuffer.CanUndo(out MoveData moveData))
            {
                moveData.PreviousState = this;
                _gameStateMachine.SetState(_resolver.Resolve<UndoState>(), moveData);
            }
        }

        public void Redo()
        {
            if (_uciBuffer.CanRedo(out MoveData moveData))
            {
                moveData.PreviousState = this;
                _gameStateMachine.SetState(_resolver.Resolve<RedoState>(), moveData);
            }
        }

        public void Pause()
        {
            _gameStateMachine.SetState(_resolver.Resolve<PauseState>());
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
            _gameStateMachine.SetState(_resolver.Resolve<EndGameState>());
            _competitors.StopPlayer();
        }

        public void Play()
        {
            // Already playing
        }
    }
}