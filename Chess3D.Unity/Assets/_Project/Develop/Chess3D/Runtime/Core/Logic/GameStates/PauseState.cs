using Chess3D.Runtime.Core.Logic.MovesBuffer;
using Chess3D.Runtime.Core.Logic.Players;
using UniRx;
using VContainer;

namespace Chess3D.Runtime.Core.Logic.GameStates
{
    public sealed class PauseState : IGameState
    {
        private UciBuffer _uciBuffer;
        private IGameStateMachine _gameStateMachine;
        private CoreEvents _coreEvents;
        private IObjectResolver _resolver;

        [Inject]
        public void Construct(UciBuffer uciBuffer, IGameStateMachine gameStateMachine, CoreEvents coreEvents, IObjectResolver resolver)
        {
            _uciBuffer = uciBuffer;
            _gameStateMachine = gameStateMachine;
            _coreEvents = coreEvents;
        }

        public string Name => "Pause";

        public void Enter()
        {
            _coreEvents.OnPause.OnNext(Unit.Default);
        }

        public void Exit()
        {
            _coreEvents.OnPlay.OnNext(Unit.Default);
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

        public void Play()
        {
            _gameStateMachine.SetState(_resolver.Resolve<IdleState>());
        }

#region unused
        public void StateUpdate()
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