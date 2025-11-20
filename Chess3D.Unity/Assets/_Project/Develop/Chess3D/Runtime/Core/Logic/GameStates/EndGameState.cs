using Chess3D.Runtime.Core.Logic.MovesBuffer;
using Chess3D.Runtime.Core.Logic.Players;
using PurrNet.StateMachine;
using VContainer;

namespace Chess3D.Runtime.Core.Logic.GameStates
{
    public class EndGameState : StateNode, IGameState
    {
        private IGameStateMachine _gameStateMachine;
        private CoreEvents _coreEvents;
        private UciBuffer _uciBuffer;
        private UndoState _undoState;

        [Inject]
        public void Construct(IGameStateMachine gameStateMachine, CoreEvents coreEvents, UciBuffer uciBuffer, UndoState undoState)
        {
            _gameStateMachine = gameStateMachine;
            _coreEvents = coreEvents;
            _uciBuffer = uciBuffer;
            _undoState = undoState;
        }

        public string Name => "End Game";

        public override void Enter()
        {
            _coreEvents.FireEnd();
        }

        public void Undo()
        {
            if (_uciBuffer.CanUndo(out MoveData moveData))
            {
                moveData.PreviousState = this;
                _gameStateMachine.SetState(_undoState, moveData);
            }
        }

#region unused

        public override void Exit()
        {
            // Empty
        }

        public override void StateUpdate()
        {
            // Empty
        }

        public void Move(string uci)
        {
            // Empty
        }

        public void Redo()
        {
            // Empty
        }

        public void Play()
        {
            // Empty
        }

        public void Pause()
        {
            // Empty
        }

#endregion
    }
}