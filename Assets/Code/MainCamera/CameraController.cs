using System.Collections;
using ChessGame.Logic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Utils.Mathematics;

#if UNITY_EDITOR
using EditorCools;
#endif

namespace MainCamera
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        private const float WhiteInitPosition = Mathf.PI*1.5f;
        private const float BlackInitialPosition = Mathf.PI*0.5f;

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

        private float _zoomCurrentVelocity;
        private float _yawCurrentVelocity;
        private float _pitchCurrentVelocity;
        private float _newDistance;
        private float _distanceMult;
        private float _newYawRad;
        private float _newPitchRad;
        private Vector3 _newPosition;
        private float _tPitch;

        private Camera _camera;

        private bool _isUpdating;
        private PieceColor _rotateCameraToColor;
        private Coroutine _autoRotateRoutine;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            Assert.IsNotNull(_camera);
        }

        public void Init(PieceColor rotateCameraToColor, Game game, bool isAutoRotationOn)
        {
            _rotateCameraToColor = rotateCameraToColor;

            if (_rotateCameraToColor == PieceColor.White)
            {
                yawRad = WhiteInitPosition;
            }
            else if (_rotateCameraToColor == PieceColor.Black)
            {
                yawRad = BlackInitialPosition;
            }

            _newDistance = distance;
            _newYawRad = yawRad;
            _newPitchRad = pitchRad;

            if (isAutoRotationOn)
            {
                game.OnEndMoveColor += AutoRotate;
            }
        }

        public void RotateToStartPosition(UnityAction continuation = null)
        {
            StartCoroutine(RotateToStartPositionRoutine(_rotateCameraToColor, continuation));
        }

        private IEnumerator RotateToStartPositionRoutine(PieceColor playerColor, UnityAction continuation)
        {
            _isUpdating = false;

            float targetYawRad = GetTargetYawRad(playerColor);

            float t = 0f;
            while (t < 1f)
            {
                CalculateTPitch();
                CalculateRotateToStartPosition(targetYawRad, ref t);
                CalculateCameraPosition();
                ApplyToCamera();

                yield return null;
            }

            _newYawRad = yawRad;
            _newPitchRad = pitchRad;
            _newDistance = distance;

            continuation?.Invoke();

            _isUpdating = true;
        }

        private void CalculateRotateToStartPosition(float targetYawRad, ref float t)
        {
            t += Time.deltaTime * 1f/initialPositionRotateTimeSec;

            yawRad = Mathf.LerpAngle(Mathf.PI, targetYawRad, Easing.Generic(t, initialPositionRotateEasing));
            pitchRad = Mathf.LerpAngle(BlackInitialPosition, Mathf.PI*0.25f, Easing.Generic(t, initialPositionRotateEasing));
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
            _newDistance -= Input.GetAxis("Scroll") * zoomSensitivity;
            // From scroll wheel
            _newDistance -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
            _newDistance = Mathf.Clamp(_newDistance, minDistance, maxDistance);

            // Smoothing zoom
            distance = Mathf.SmoothDamp(distance, _newDistance, ref _zoomCurrentVelocity, zoomSmoothTime);
        }

        private void CalculateOrbit()
        {
            if (Input.GetButton("Fire2"))
            {
                _newYawRad -= Input.GetAxis("Mouse X") * orbitSensitivity;
                _newPitchRad -= Input.GetAxis("Mouse Y") * orbitSensitivity;
            }

            _newYawRad += Input.GetAxis("Horizontal") * orbitSensitivity;
            _newPitchRad += Input.GetAxis("Vertical") * orbitSensitivity;
            _newPitchRad = Mathf.Clamp(_newPitchRad, minPitchRad, maxPitchRad);

            // Smoothing orbit
            yawRad = Mathf.SmoothDamp(yawRad, _newYawRad, ref _yawCurrentVelocity, yawSmoothTime);
            pitchRad = Mathf.SmoothDamp(pitchRad, _newPitchRad, ref _pitchCurrentVelocity, pitchSmoothTime);
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
            _camera.fieldOfView = Mathf.Lerp(fovStart, fovEnd, _tPitch);
        }

        public void AutoRotate(PieceColor color)
        {
            AutoRotate(color, null);
        }

        public void AutoRotate(PieceColor color, UnityAction continuation)
        {
            if (_autoRotateRoutine != null)
            {
                StopCoroutine(_autoRotateRoutine);
            }
            _autoRotateRoutine = StartCoroutine(AutoRotateRoutine(color, continuation));
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

        /// Start position for certain color
        private static float GetTargetYawRad(PieceColor playerColor)
        {
            return playerColor switch
            {
                PieceColor.White => WhiteInitPosition, // 270 degrees
                PieceColor.Black => BlackInitialPosition, // 90 degrees
                _ => WhiteInitPosition,
            };
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

        private void RotateYawFromTo(float startYawRad, float endYawRad, ref float t)
        {
            t += Time.deltaTime * 1f/autoRotateTimeSec;
            float start = Mathf.Rad2Deg * startYawRad;
            float end = Mathf.Rad2Deg * endYawRad;
            yawRad = Mathf.Deg2Rad * Mathf.LerpAngle(start, end, Easing.Generic(t, autoRotateEasing));
        }

#if UNITY_EDITOR

        [Button(space: 20, row:"0")]
        public void AutoRotateWhiteButton()
        {
            AutoRotate(PieceColor.White);
        }

        [Button(space: 10, row:"0")]
        public void AutoRotateBlackButton()
        {
            AutoRotate(PieceColor.Black);
        }

        [Button(space: 10)]
        public void RotateToStartPositionButton()
        {
           RotateToStartPosition();
        }
#endif
    }
}
