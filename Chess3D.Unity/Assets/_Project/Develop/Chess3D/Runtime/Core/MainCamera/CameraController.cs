using System.Collections;
using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.InputManagement;
using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Notation;
using Chess3D.Runtime.Utilities;
using Chess3D.Runtime.Utilities.Common.Mathematics;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace Chess3D.Runtime.Core.MainCamera
{
    public class CameraController : MonoBehaviour, ILoadUnit
    {
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

        private Game _game;
        private Camera _mainCamera;
        private SettingsService _settingsService;
        private CoreEvents _coreEvents;
        private FenFromString _fenFromString;

        private bool _isUpdating;
        private bool _isStartRotationOn;
        private PieceColor _rotateCameraToColor;
        private Coroutine _startRotateRoutine;

        private float _whiteInitialYawRad;
        private float _blackInitialYawRad;
        private float _zoomCurrentVelocity;
        private float _yawCurrentVelocity;
        private float _pitchCurrentVelocity;
        private float _newDistance;
        private float _distanceMult;
        private float _newYawRad;
        private float _newPitchRad;
        private Vector3 _newPosition;
        private float _tPitch;

        private bool IsVsPlayer => _settingsService.S.GameSettings.PlayerWhite.PlayerType == PlayerType.Human
                                   && _settingsService.S.GameSettings.PlayerBlack.PlayerType == PlayerType.Human;

        [Inject]
        public void Construct(Game game, [Key(CameraKeys.Main)] Camera mainCamera,
            SettingsService settingsService, CoreEvents coreEvents, FenFromString fenFromString)
        {
            _game = game;
            _mainCamera = mainCamera;
            _settingsService = settingsService;
            _coreEvents = coreEvents;
            _fenFromString = fenFromString;

            _coreEvents.OnEndMove += AutoRotate;
            _coreEvents.OnEnd += StopAutoRotate;
        }

        private void OnDestroy()
        {
            if (_coreEvents is null)
            {
                return;
            }

            _coreEvents.OnEndMove -= AutoRotate;
            _coreEvents.OnEnd -= StopAutoRotate;
        }

        public UniTask Load()
        {
            _isStartRotationOn = IsVsPlayer && _settingsService.S.GameSettings.IsAutoRotateCamera;
            _rotateCameraToColor = IsVsPlayer ? _fenFromString.TurnColor : _settingsService.S.GameSettings.PlayerColor;
            // TODO: offline-online camera autoRotation
            // _isStartRotationOn = IsVsPlayer && OnlineInstanceHandler.IsOffline && gameSettingsContainer.IsAutoRotateCamera;

            _newDistance = distance;
            _newYawRad = yawRad;
            _newPitchRad = pitchRad;

            if (_rotateCameraToColor == PieceColor.White)
            {
                _whiteInitialYawRad = Mathf.PI*1.5f;
                _blackInitialYawRad = Mathf.PI*0.5f;
            }
            else if (_rotateCameraToColor == PieceColor.Black)
            {
                _whiteInitialYawRad = Mathf.PI*0.5f;
                _blackInitialYawRad = Mathf.PI*1.5f;
            }

            if (_rotateCameraToColor == PieceColor.White)
            {
                yawRad = _whiteInitialYawRad;
            }
            else if (_rotateCameraToColor == PieceColor.Black)
            {
                yawRad = _blackInitialYawRad;
            }

            return UniTask.CompletedTask;
        }

        public async UniTask RotateToStartPosition()
        {
            _isUpdating = false;

            float targetYawRad = GetTargetYawRad(_rotateCameraToColor);

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

        private void CalculateOrbit()
        {
            if (InputController.Rmb())
            {
                _newYawRad -= InputController.Horizontal() * orbitSensitivity;
                _newPitchRad -= InputController.Vertical() * orbitSensitivity;
            }

            _newPitchRad = Mathf.Clamp(_newPitchRad, minPitchRad, maxPitchRad);

            // Smoothing orbit
            yawRad = Mathf.SmoothDamp(yawRad, _newYawRad, ref _yawCurrentVelocity, yawSmoothTime);
            pitchRad = Mathf.SmoothDamp(pitchRad, _newPitchRad, ref _pitchCurrentVelocity, pitchSmoothTime);
        }

        public void AutoRotate()
        {
            if (!_settingsService.S.GameSettings.IsAutoRotateCamera)
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
                PieceColor.White => _whiteInitialYawRad,
                PieceColor.Black => _blackInitialYawRad,
                _ => _whiteInitialYawRad,
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