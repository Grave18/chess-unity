using Chess3D.Runtime.Core.Logic.MovesBuffer;
using Chess3D.Runtime.Core.Logic.Players;
using Chess3D.Runtime.Core.Sound;
using UniRx;
using VContainer;

namespace Chess3D.Runtime.Core.Logic.GameStates
{
    public sealed class EndGameState : IGameState
    {
        private IGameStateMachine _gameStateMachine;
        private CoreEvents _coreEvents;
        private UciBuffer _uciBuffer;
        private EffectsPlayer _effectsPlayer;
        private IObjectResolver _resolver;

        [Inject]
        public void Construct(IGameStateMachine gameStateMachine, CoreEvents coreEvents,
            UciBuffer uciBuffer, EffectsPlayer effectsPlayer, IObjectResolver resolver)
        {
            _gameStateMachine = gameStateMachine;
            _coreEvents = coreEvents;
            _uciBuffer = uciBuffer;
            _effectsPlayer = effectsPlayer;
            _resolver = resolver;
        }

        public string Name => "End Game";

        public void Enter()
        {
            _effectsPlayer.PlayGameEndSound();
            _coreEvents.OnEnd.OnNext(Unit.Default);
        }

        public void Undo()
        {
            if (_uciBuffer.CanUndo(out MoveData moveData))
            {
                moveData.PreviousState = _resolver.Resolve<IdleState>();
                _gameStateMachine.SetState<UndoState, MoveData>(moveData);
            }
        }

#region unused

        public void Exit()
        {
            // Empty
        }

        public void StateUpdate()
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