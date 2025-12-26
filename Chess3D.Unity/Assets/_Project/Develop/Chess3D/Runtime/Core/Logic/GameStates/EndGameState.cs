using Chess3D.Runtime.Core.Logic.MovesBuffer;
using Chess3D.Runtime.Core.Logic.Players;
using Chess3D.Runtime.Core.Sound;
using PurrNet.StateMachine;
using UniRx;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.Logic.GameStates
{
    public sealed class EndGameState : StateNode, IGameState
    {
        [SerializeField] private UndoState undoState;
        [SerializeField] private IdleState idleState;

        private IGameStateMachine _gameStateMachine;
        private CoreEvents _coreEvents;
        private UciBuffer _uciBuffer;
        private EffectsPlayer _effectsPlayer;

        [Inject]
        public void Construct(IGameStateMachine gameStateMachine, CoreEvents coreEvents,
            UciBuffer uciBuffer, EffectsPlayer effectsPlayer)
        {
            _gameStateMachine = gameStateMachine;
            _coreEvents = coreEvents;
            _uciBuffer = uciBuffer;
            _effectsPlayer = effectsPlayer;
        }

        public string Name => "End Game";

        public override void Enter()
        {
            _effectsPlayer.PlayGameEndSound();
            _coreEvents.OnEnd.OnNext(Unit.Default);
        }

        public void Undo()
        {
            if (_uciBuffer.CanUndo(out MoveData moveData))
            {
                moveData.PreviousState = idleState;
                _gameStateMachine.SetState(undoState, moveData);
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