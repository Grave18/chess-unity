using UnityEngine;

namespace MainCamera
{
    public class CameraController : MonoBehaviour
    {
        [Tooltip("Can be null if you want to look at the world origin")]
        [SerializeField] private Transform target;

        [Header("Mouse")]
        [Header("Zoom")]
        [Tooltip("Camera distance from the target")]
        [SerializeField] private float distance = 3.5f;
        [SerializeField] private float minDistance = 1f;
        [SerializeField] private float maxDistance = 4f;
        [SerializeField] private float zoomScrollWheelSensitivity = 2.5f;
        [SerializeField] private float zoomKeyboardSensitivity = 0.2f;
        [SerializeField] private float zoomSmoothTime = 0.15f;

        [Header("Orbiting")]
        [SerializeField] private float yawRad;
        [SerializeField] private float pitchRad;
        [SerializeField] private float minPitchRad = 0.1f;
        [SerializeField] private float maxPitchRad = 1f;
        [SerializeField] private float orbitSensitivity = 10f;
        [SerializeField] private float yawSmoothTime = 0.15f;
        [SerializeField] private float pitchSmoothTime = 0.15f;

        private float _zoomCurrentVelocity;
        private float _yawCurrentVelocity;
        private float _pitchCurrentVelocity;
        private float _newDistance;
        private float _newYawRad;
        private float _newPitchRad;

        private void Start()
        {
            _newDistance = distance;
            _newYawRad = yawRad;
            _newPitchRad = pitchRad;
        }

        private void Update()
        {
            // Position
            Vector3 pos = transform.position;

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

            // Zoom
            // From keyboard
            _newDistance -= Input.GetAxis("Scroll") * zoomKeyboardSensitivity;
            // From scroll wheel
            _newDistance -= Input.GetAxis("Mouse ScrollWheel") * zoomScrollWheelSensitivity;
            _newDistance = Mathf.Clamp(_newDistance, minDistance, maxDistance);

            // Smoothing zoom
            distance = Mathf.SmoothDamp(distance, _newDistance, ref _zoomCurrentVelocity, zoomSmoothTime);

            pos.x = distance * Mathf.Cos(yawRad) * Mathf.Cos(pitchRad);
            pos.y = distance * Mathf.Sin(pitchRad);
            pos.z = distance * Mathf.Sin(yawRad) * Mathf.Cos(pitchRad);

            transform.position = pos;

            // Rotation
            if (target != null)
            {
                transform.LookAt(target);
            }
            else
            {
                transform.LookAt(Vector3.zero);
            }
        }
    }
}
