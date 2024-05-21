using UnityEngine;

namespace View
{
    public class CameraController : MonoBehaviour
    {
        [Tooltip("Can be null if you want to look at the world origin")]
        [SerializeField] private Transform target;

        [Header("Mouse")]
        [Tooltip("Mouse sensitivity")]
        [SerializeField] private float sensitivity = 10f;
        [Tooltip("Mouse sensitivity")]
        [SerializeField] private float scrollSensitivity = 100f;

        [Header("Distance from target")]
        [Tooltip("Camera distance from the target")]
        [SerializeField] private float distance = 4f;
        [SerializeField] private float minDistance = 1f;
        [SerializeField] private float maxDistance = 4f;

        [Header("Pitch")]
        [SerializeField] private float minPitchRad = 0.1f;
        [SerializeField] private float maxPitchRad = 1f;

        private float _yaw;
        private float _pitch;

        private void Update()
        {
            // Position
            var pos = transform.position;

            float dt = Time.deltaTime;

            if (Input.GetButton("Fire2"))
            {
                _yaw -= Input.GetAxis("Mouse X") * sensitivity * dt;
                _pitch -= Input.GetAxis("Mouse Y") * sensitivity * dt;
            }

            _yaw += Input.GetAxis("Horizontal") * sensitivity * dt;
            _pitch += Input.GetAxis("Vertical") * sensitivity * dt;

            _pitch = Mathf.Clamp(_pitch, minPitchRad, maxPitchRad);

            // Scroll
            distance -= Input.GetAxis("Scroll") * scrollSensitivity * dt;
            distance -= Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            pos.x = distance * Mathf.Cos(_yaw) * Mathf.Cos(_pitch);
            pos.y = distance * Mathf.Sin(_pitch);
            pos.z = distance * Mathf.Sin(_yaw) * Mathf.Cos(_pitch);

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
