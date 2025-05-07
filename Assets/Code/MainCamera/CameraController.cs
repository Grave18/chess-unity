using System.Collections;
using ChessGame.Logic;
using EditorCools;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Utils.Mathematics;

namespace MainCamera
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Vector3 offset = new(0, 0.15f, 0);

        [Header("Zoom")]
        [Tooltip("Camera distance from the target")]
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

        [Header("Auto Rotate")]
        [SerializeField] private float autoRotateSpeed = 1f;
        [SerializeField] private EasingType easingOfAutoRotation = EasingType.InOutQuad;

        private float _zoomCurrentVelocity;
        private float _yawCurrentVelocity;
        private float _pitchCurrentVelocity;
        private float _newDistance;
        private float _distanceMult;
        private float _newYawRad;
        private float _newPitchRad;
        private Vector3 _newPosition;
        private float _t;

        private Camera _camera;

        private bool _isInitialized;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            Assert.IsNotNull(_camera);
        }

        public void Init(PieceColor color)
        {
            if (color == PieceColor.White)
            {
                yawRad = -1.5708f;
            }
            else if (color == PieceColor.Black)
            {
                yawRad = 1.5708f;
            }

            _newDistance = distance;
            _newYawRad = yawRad;
            _newPitchRad = pitchRad;

            RotateToStartPosition(color, () => _isInitialized = true);
        }

        [Button(space: 10, args: new object[] { PieceColor.White, null })]
        private void RotateToStartPosition(PieceColor color, UnityAction continuation)
        {
            StartCoroutine(Rotate(color, continuation));
        }

        private IEnumerator Rotate(PieceColor playerColor, UnityAction continuation)
        {
            float targetYawRad = playerColor switch
            {
                PieceColor.White => -1.5708f,
                PieceColor.Black => -4.71f,
                _ => -1.5708f,
            };

            float t = 0f;

            while (t < 1f)
            {
                CalculateTFromPitch();

                t += Time.deltaTime * autoRotateSpeed;

                yawRad = Mathf.Lerp(-3.14f, targetYawRad, Easing.Generic(t, easingOfAutoRotation));
                pitchRad = Mathf.Lerp(1.56f, 0.73f, Easing.Generic(t, easingOfAutoRotation));
                distance = Mathf.Lerp(3f, 1.25f, Easing.Generic(t, easingOfAutoRotation));

                CalculateCameraPosition();
                ApplyToCamera();

                yield return null;
            }

            _newYawRad = yawRad;
            _newPitchRad = pitchRad;
            _newDistance = distance;

            continuation?.Invoke();
        }

        private void Update()
        {
            if (!_isInitialized)
            {
                return;
            }

            CalculateTFromPitch();
            CalculateZoom();
            CalculateOrbit();
            CalculateCameraPosition();
            ApplyToCamera();
        }

        /// t = min pitch -> 0 max pitch -> 1
        private void CalculateTFromPitch()
        {
            _t = (pitchRad - minPitchRad) / (maxPitchRad - minPitchRad);
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
            _distanceMult = Mathf.Lerp(1f, distanceFovMultEnd, _t);
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
            _camera.fieldOfView = Mathf.Lerp(fovStart, fovEnd, _t);
        }
    }
}
