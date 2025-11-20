using Chess3D.Runtime.Core.Logic.MovesBuffer;
using Chess3D.Runtime.Core.Logic.Players;
using PurrNet.StateMachine;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.Logic.GameStates
{
    public class IdleState : StateNode, IGameState
    {
        private Game _game;
        private Competitors _competitors;
        private UciBuffer _uciBuffer;
        private CoreEvents _coreEvents;
        private IGameStateMachine _gameStateMachine;

        [SerializeField] private EndGameState endGameState;
        [SerializeField] private PauseState pauseState;
        [SerializeField] private MoveState moveState;
        [SerializeField] private UndoState undoState;
        [SerializeField] private RedoState redoState;

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


        public override void Enter()
        {
            Debug.Log("State: " + Name);

            if (_game.IsEndGame)
            {
                EndGame();
                return;
            }

            _isRunning = true;
            _competitors.StartPlayer();

            _coreEvents.FireIdle();
        }

        public override void Exit()
        {
            _isRunning = false;
            _competitors.StopPlayer();
        }

        public void Move(string uci)
        {
            _gameStateMachine.SetState(moveState, uci);
        }

        public void Undo()
        {
            if (_uciBuffer.CanUndo(out MoveData moveData))
            {
                moveData.PreviousState = this;
                _gameStateMachine.SetState(undoState, moveData);
            }
        }

        public void Redo()
        {
            if (_uciBuffer.CanRedo(out MoveData moveData))
            {
                moveData.PreviousState = this;
                _gameStateMachine.SetState(redoState, moveData);
            }
        }

        public void Play()
        {
            // Already playing
        }

        public void Pause()
        {
            _gameStateMachine.SetState(pauseState);
        }

        public override void StateUpdate()
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
            _gameStateMachine.SetState(endGameState);
            _competitors.StopPlayer();
        }
    }
}