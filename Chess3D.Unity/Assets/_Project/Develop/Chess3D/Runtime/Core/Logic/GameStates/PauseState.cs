using Chess3D.Runtime.Core.Logic.MovesBuffer;
using Chess3D.Runtime.Core.Logic.Players;
using PurrNet.StateMachine;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.Logic.GameStates
{
    public class PauseState : StateNode,IGameState
    {
        private UciBuffer _uciBuffer;
        private IGameStateMachine _gameStateMachine;
        private CoreEvents _coreEvents;

        [SerializeField] private IdleState idleState;
        [SerializeField] private UndoState undoState;
        [SerializeField] private RedoState redoState;

        [Inject]
        public void Construct(UciBuffer uciBuffer, IGameStateMachine gameStateMachine, CoreEvents coreEvents)
        {
            _uciBuffer = uciBuffer;
            _gameStateMachine = gameStateMachine;
            _coreEvents = coreEvents;
        }

        public string Name => "Pause";

        public override void Enter()
        {
            Debug.Log("State: " + Name);

            _coreEvents.FirePause();
        }

        public override void Exit()
        {
            _coreEvents.FirePlay();
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
            _gameStateMachine.SetState(idleState);
        }

#region unused
        public override void StateUpdate()
        {
            // Nothing to update
        }

        public void Move(string uci)
        {
            // Can't move from Pause
        }

        public void Pause()
        {
            // Already paused
        }
#endregion
    }
}