using Chess3D.Runtime.Core.Logic.Players;
using Chess3D.Runtime.Core.MainCamera;
using Cysharp.Threading.Tasks;
using PurrNet.StateMachine;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.Logic.GameStates
{
    public class WarmUpState : StateNode, IGameState
    {
        private IGameStateMachine _gameStateMachine;
        private SettingsService _settingsService;
        private CameraController _cameraController;
        private CoreEvents _coreEvents;

        [SerializeField] private IdleState idleState;

        private const float WarmupTimeSec = 3f; // TODO: Make configurable
        private float t;

        public string Name => "WarmUp";

        [Inject]
        public void Construct(IGameStateMachine gameStateMachine, SettingsService settingsService,
            CameraController cameraController, CoreEvents coreEvents)
        {
            _gameStateMachine = gameStateMachine;
            _settingsService = settingsService;
            _cameraController = cameraController;
            _coreEvents = coreEvents;
        }

        public override void Enter()
        {
            Debug.Log("State: " + Name);

            t = WarmupTimeSec;

            _cameraController.RotateToStartPosition().Forget();

            _coreEvents.FireWarmup();
        }

        public override void Exit()
        {
            _coreEvents.FireStart();
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

        // Unused
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
    }
}