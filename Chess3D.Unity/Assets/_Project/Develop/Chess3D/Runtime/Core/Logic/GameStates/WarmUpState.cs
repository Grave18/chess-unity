using Chess3D.Runtime.Core.Logic.Players;
using Chess3D.Runtime.Core.Sound;
using PurrNet.StateMachine;
using UnityEngine;
using VContainer;
using UniRx;

namespace Chess3D.Runtime.Core.Logic.GameStates
{
    public sealed class WarmUpState : StateNode, IGameState
    {
        private IGameStateMachine _gameStateMachine;
        private CoreEvents _coreEvents;
        private ConfigContainer _configContainer;
        private EffectsPlayer _effectsPlayer;

        [SerializeField] private IdleState idleState;

        private float t;

        public string Name => "WarmUp";

        private bool _isConstructed;

        [Inject]
        public void Construct(IGameStateMachine gameStateMachine, CoreEvents coreEvents,
            ConfigContainer configContainer, EffectsPlayer effectsPlayer)
        {
            _gameStateMachine = gameStateMachine;
            _coreEvents = coreEvents;
            _configContainer = configContainer;
            _effectsPlayer = effectsPlayer;
            _isConstructed = true;
        }

        public override void Enter()
        {
            if (!_isConstructed)
            {
                return;
            }

            t = _configContainer.ChessConfig.WarmupTimeSec;

            _coreEvents.OnWarmup.OnNext(Unit.Default);
        }

        public override void Exit()
        {
            _effectsPlayer.PlayGameStartSound();
            _coreEvents.OnStart.OnNext(Unit.Default);
        }

        public override void StateUpdate()
        {
            if (!_isConstructed)
            {
                return;
            }

            if (t <= 0f)
            {
                return;
            }

            t -= Time.deltaTime;

            if (t <= 0f)
            {
                _gameStateMachine.SetState(idleState);
            }
        }

#region unused
        public void Move(string uci)
        {
            // Empty
        }

        public void Undo()
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
#endregion unused
    }
}