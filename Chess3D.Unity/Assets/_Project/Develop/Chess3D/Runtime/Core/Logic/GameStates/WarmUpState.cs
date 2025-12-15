using Chess3D.Runtime.Core.Logic.Players;
using Chess3D.Runtime.Core.MainCamera;
using Cysharp.Threading.Tasks;
using PurrNet.StateMachine;
using UniRx;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.Logic.GameStates
{
    public class WarmUpState : StateNode, IGameState
    {
        private IGameStateMachine _gameStateMachine;
        private CameraController _cameraController;
        private CoreEvents _coreEvents;

        [SerializeField] private IdleState idleState;

        private const float WarmupTimeSec = 3f; // TODO: Make configurable
        private float t;

        public string Name => "WarmUp";

        [Inject]
        public void Construct(IGameStateMachine gameStateMachine,
            CameraController cameraController, CoreEvents coreEvents)
        {
            _gameStateMachine = gameStateMachine;
            _cameraController = cameraController;
            _coreEvents = coreEvents;
        }

        public override void Enter()
        {
            t = WarmupTimeSec;

            _cameraController.RotateToStartPosition().Forget();

            _coreEvents.OnWarmup.OnNext(Unit.Default);
        }

        public override void Exit()
        {
            _coreEvents.OnStart.OnNext(Unit.Default);
        }

        public override void StateUpdate()
        {
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