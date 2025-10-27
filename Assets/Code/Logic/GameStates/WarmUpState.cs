using Cysharp.Threading.Tasks;
using MainCamera;
using PurrNet.StateMachine;
using Settings;
using TNRD;
using UnityEngine;

namespace Logic.GameStates
{
    public class WarmUpState : StateNode, IGameState
    {
        [Header("References")]
        [SerializeField] private Game game;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private CameraController cameraController;

        [Header("States")]
        [SerializeField] private SerializableInterface<IGameState> idleState;

        [Header("Settings")]
        [SerializeField] private float warmupTimeSec = 3f;

        private float t;

        public string Name => "WarmUp";

        public override void Enter()
        {
            Debug.Log("State: " + Name);

            t = warmupTimeSec;

            bool isRotateCameraOnStart = gameSettingsContainer.IsRotateCameraOnStart;
            cameraController.RotateToStartPosition(isRotateCameraOnStart).Forget();

            game.FireWarmup();
        }

        public override void Exit()
        {
            game.FireStart();
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
                game.GameStateMachine.SetState(idleState.Value);
            }
        }

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