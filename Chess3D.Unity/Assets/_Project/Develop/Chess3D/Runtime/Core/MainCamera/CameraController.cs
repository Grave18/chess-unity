using System.Collections;
using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.InputManagement;
using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Notation;
using Chess3D.Runtime.Online;
using Chess3D.Runtime.Utilities;
using Chess3D.Runtime.Utilities.Common.Mathematics;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace Chess3D.Runtime.Core.MainCamera
{
    public sealed class CameraController : MonoBehaviour, ILoadUnit
    {
        private const float WhiteInitialYawRad = Mathf.PI*1.5f;
        private const float BlackInitialYawRad = Mathf.PI*0.5f;

        // TODO: Move LayerMask to SO
        private const float MaxDistance = 100;
        private LayerMask _layerMask;

        // TODO: move some CameraController values in config
        [Header("Target")]
        [SerializeField] private Vector3 offset = new(0, 0.15f, 0);

        [Header("Zoom")]
        [SerializeField] private float minDistance = 1f;
        [SerializeField] private float maxDistance = 2.5f;
        [SerializeField] private float distance = 1.5f;
        [SerializeField] private float zoomSensitivity = 2.5f;
        [SerializeField] private float zoomSmoothTime = 0.15f;

        [Header("Orbiting")]
        [SerializeField] private float minPitchRad = 0.1f;
        [SerializeField] private float maxPitchRad = 1.56f;
        [SerializeField] private float pitchRad = 0.125f;
        [SerializeField] private float yawRad = -1.5708f;
        [SerializeField] private float orbitSensitivity = 0.2f;
        [SerializeField] private float yawSmoothTime = 0.08f;
        [SerializeField] private float pitchSmoothTime = 0.08f;

        [Header("Fov")]
        [SerializeField] private float fovStart = 60f;
        [SerializeField] private float fovEnd = 30f;
        [SerializeField] private float distanceFovMultEnd = 1.5f;

        [Header("Rotate to start position")]
        [SerializeField] private float initialPositionRotateTimeSec = 2f;
        [SerializeField] private EasingType initialPositionRotateEasing = EasingType.InOutQuad;

        [Header("Auto rotation")]
        [SerializeField] private float autoRotateTimeSec = 0.5f;
        [SerializeField] private EasingType autoRotateEasing = EasingType.InOutQuad;

        private bool _isUpdating;
        private bool _isStartRotationOn;
        private bool _isAutoRotateCamera;
        private PieceColor _rotateCameraToColor;
        private Coroutine _startRotateRoutine;

        private float _zoomCurrentVelocity;
        private float _yawCurrentVelocity;
        private float _pitchCurrentVelocity;
        private float _newDistance;
        private float _distanceMult;
        private float _newYawRad;
        private float _newPitchRad;
        private Vector3 _newPosition;
        private float _tPitch;

        private Game _game;
        private Camera _mainCamera;
        private ISettingsContainer _settingsContainer;
        private FenFromString _fenFromString;
        private ConfigContainer _configContainer;

        private bool IsVsPlayer => _settingsContainer.S.GameSettings.PlayerWhite.PlayerType == PlayerType.Human
                                   && _settingsContainer.S.GameSettings.PlayerBlack.PlayerType == PlayerType.Human;

        [Inject]
        public void Construct(Game game, [Key(CameraKeys.Main)] Camera mainCamera,
            ISettingsContainer settingsContainer, CoreEvents coreEvents, FenFromString fenFromString,
            ConfigContainer configContainer)
        {
            _game = game;
            _mainCamera = mainCamera;
            _settingsContainer = settingsContainer;
            _fenFromString = fenFromString;
            _layerMask = LayerMask.GetMask("Piece", "Square");
            _configContainer = configContainer;

            coreEvents.OnEndMove.Subscribe(_ => AutoRotate()).AddTo(this);
            coreEvents.OnEnd.Subscribe(_ => StopAutoRotate()).AddTo(this);
        }

        public UniTask Load()
        {
            _rotateCameraToColor = IsVsPlayer ? _fenFromString.TurnColor : _settingsContainer.S.GameSettings.PlayerColor;
            _isStartRotationOn = _settingsContainer.S.GameSettings.IsRotateCameraOnStart;
            _isAutoRotateCamera = IsVsPlayer
                                  && _configContainer.OnlineConfig.ConnectionType == ConnectionType.Offline
                                  && _settingsContainer.S.GameSettings.IsAutoRotateCamera;

            _newDistance = distance;
            _newYawRad = yawRad;
            _newPitchRad = pitchRad;

            return UniTask.CompletedTask;
        }

        public async UniTask RotateToStartPosition()
        {
            _isUpdating = false;

            float targetYawRad = GetTargetYawRad(_rotateCameraToColor);
            yawRad = targetYawRad;

            float t = 0f;
            while (t < 1f)
            {
                if (!_isStartRotationOn)
                {
                    t = 1f;
                }

                CalculateTPitch();
                CalculateRotateToStartPosition(targetYawRad, ref t);
                CalculateCameraPosition();
                ApplyToCamera();

                await UniTask.NextFrame();
            }

            _newYawRad = yawRad;
            _newPitchRad = pitchRad;
            _newDistance = distance;

            _isUpdating = true;
        }

        private void CalculateRotateToStartPosition(float targetYawRad, ref float t)
        {
            t += Time.deltaTime * 1f/initialPositionRotateTimeSec;

            yawRad = Mathf.LerpAngle(Mathf.PI, targetYawRad, Easing.Generic(t, initialPositionRotateEasing));
            pitchRad = Mathf.LerpAngle(Mathf.PI*0.5f, Mathf.PI*0.25f, Easing.Generic(t, initialPositionRotateEasing));
            distance = Mathf.Lerp(3f, 1.25f, Easing.Generic(t, initialPositionRotateEasing));
        }

        private void Update()
        {
            if (!_isUpdating)
            {
                return;
            }

            CalculateTPitch();
            CalculateZoom();
            CalculateOrbit();
            CalculateCameraPosition();
            ApplyToCamera();
        }

        /// t = min pitch -> 0 max pitch -> 1
        private void CalculateTPitch()
        {
            _tPitch = (pitchRad - minPitchRad) / (maxPitchRad - minPitchRad);
        }

        private void CalculateZoom()
        {
            // From keyboard
            _newDistance -= InputController.Scroll() * zoomSensitivity;
            _newDistance = Mathf.Clamp(_newDistance, minDistance, maxDistance);

            // Smoothing zoom
            distance = Mathf.SmoothDamp(distance, _newDistance, ref _zoomCurrentVelocity, zoomSmoothTime);
        }

        public bool IsHitSomething(out ISelectable selected)
        {
            selected = null;
            Vector3 mousePos = InputController.MousePosition();
            Ray ray = _mainCamera.ScreenPointToRay(mousePos);
            Physics.Raycast(ray, out RaycastHit hit, MaxDistance, _layerMask);
            Transform hitTransform = hit.transform;

            if (InputController.GetLmbDown() && hitTransform != null)
            {
                hitTransform.TryGetComponent(out selected);
                return true;
            }

            return false;
        }

        private void CalculateOrbit()
        {
            DisableOrbitIfHitChessPiece();

            if (InputController.GetRmb() || InputController.GetLmb())
            {
                _newYawRad -= InputController.Horizontal() * orbitSensitivity;
                _newPitchRad -= InputController.Vertical() * orbitSensitivity;
            }

            _newPitchRad = Mathf.Clamp(_newPitchRad, minPitchRad, maxPitchRad);

            // Smoothing orbit
            yawRad = Mathf.SmoothDamp(yawRad, _newYawRad, ref _yawCurrentVelocity, yawSmoothTime);
            pitchRad = Mathf.SmoothDamp(pitchRad, _newPitchRad, ref _pitchCurrentVelocity, pitchSmoothTime);
        }

        private void DisableOrbitIfHitChessPiece()
        {
            if (InputController.GetLmbDown() && IsHitSomething(out ISelectable selected) && _game.CanSelect(selected))
            {
                InputController.IsOrbitEnabled = false;
            }
            else if (InputController.GetLmbUp())
            {
                InputController.IsOrbitEnabled = true;
            }
        }

        public void AutoRotate()
        {
            if (!_isAutoRotateCamera)
            {
                return;
            }

            AutoRotate(_game.CurrentTurnColor, null);
        }

        public void AutoRotate(PieceColor color, UnityAction continuation)
        {
            StopAutoRotate();
            _startRotateRoutine = StartCoroutine(AutoRotateRoutine(color, continuation));
        }

        private void StopAutoRotate()
        {
            if(_startRotateRoutine != null)
            {
                _isUpdating = true;
                StopCoroutine(_startRotateRoutine);
            }
        }

        private IEnumerator AutoRotateRoutine(PieceColor color, UnityAction continuation)
        {
            _isUpdating = false;

            ClampYawRadTo_0_360();

            float startYawRad = yawRad;
            float endYawRad = GetTargetYawRad(color);

            float t = 0;
            while (t < 1f)
            {
                RotateYawFromTo(startYawRad, endYawRad, ref t);
                CalculateCameraPosition();
                ApplyToCamera();

                yield return null;
            }

            _newYawRad = yawRad;

            continuation?.Invoke();

            _isUpdating = true;
        }

        private void ClampYawRadTo_0_360()
        {
            yawRad %= Mathf.PI * 2f;

            // Convert negative angle
            if (yawRad < 0)
            {
                yawRad = 2*Mathf.PI + yawRad;
            }
        }

        /// Get start position for black or white color
        private float GetTargetYawRad(PieceColor playerColor)
        {
            return playerColor switch
            {
                PieceColor.White => WhiteInitialYawRad,
                PieceColor.Black => BlackInitialYawRad,
                _ => WhiteInitialYawRad,
            };
        }

        private void RotateYawFromTo(float startYawRad, float endYawRad, ref float t)
        {
            t += Time.deltaTime * 1f/autoRotateTimeSec;
            float start = Mathf.Rad2Deg * startYawRad;
            float end = Mathf.Rad2Deg * endYawRad;
            yawRad = Mathf.Deg2Rad * Mathf.LerpAngle(start, end, Easing.Generic(t, autoRotateEasing));
        }

        private void CalculateCameraPosition()
        {
            // Less fov more distance from target to camera
            _distanceMult = Mathf.Lerp(1f, distanceFovMultEnd, _tPitch);
            float currentDistance = distance * _distanceMult;

            // Position
            _newPosition.x = currentDistance * Mathf.Cos(yawRad) * Mathf.Cos(pitchRad) + offset.x;
            _newPosition.y = currentDistance * Mathf.Sin(pitchRad) + offset.y;
            _newPosition.z = currentDistance * Mathf.Sin(yawRad) * Mathf.Cos(pitchRad) + offset.z;
        }

        private void ApplyToCamera()
        {
            transform.position = _newPosition;
            transform.LookAt(offset);
            _mainCamera.fieldOfView = Mathf.Lerp(fovStart, fovEnd, _tPitch);
        }
    }
}