using Chess3D.Runtime.Core.Logic.Players;
using Chess3D.Runtime.Core.Sound;
using UnityEngine;
using VContainer;
using UniRx;

namespace Chess3D.Runtime.Core.Logic.GameStates
{
    public sealed class WarmUpState : IGameState
    {
        private IGameStateMachine _gameStateMachine;
        private CoreEvents _coreEvents;
        private ConfigContainer _configContainer;
        private EffectsPlayer _effectsPlayer;

        private float t;

        public string Name => "WarmUp";

        [Inject]
        public void Construct(IGameStateMachine gameStateMachine, CoreEvents coreEvents,
            ConfigContainer configContainer, EffectsPlayer effectsPlayer)
        {
            _gameStateMachine = gameStateMachine;
            _coreEvents = coreEvents;
            _configContainer = configContainer;
            _effectsPlayer = effectsPlayer;
        }

        public void Enter()
        {
            t = _configContainer.ChessConfig.WarmupTimeSec;

            _coreEvents.OnWarmup.OnNext(Unit.Default);
        }

        public void Exit()
        {
            _effectsPlayer.PlayGameStartSound();
            _coreEvents.OnStart.OnNext(Unit.Default);
        }

        public void StateUpdate()
        {
            if (t <= 0f)
            {
                return;
            }

            t -= Time.deltaTime;

            if (t <= 0f)
            {
                _gameStateMachine.SetState<IdleState>();
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