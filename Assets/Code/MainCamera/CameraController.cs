using UnityEngine;

namespace MainCamera
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
        [SerializeField] private float yawRad;
        [SerializeField] private float pitchRad;
        [SerializeField] private float minPitchRad = 0.1f;
        [SerializeField] private float maxPitchRad = 1f;


        private void Update()
        {
            // Position
            var pos = transform.position;

            float dt = Time.deltaTime;

            if (Input.GetButton("Fire2"))
            {
                yawRad -= Input.GetAxis("Mouse X") * sensitivity * dt;
                pitchRad -= Input.GetAxis("Mouse Y") * sensitivity * dt;
            }

            yawRad += Input.GetAxis("Horizontal") * sensitivity * dt;
            pitchRad += Input.GetAxis("Vertical") * sensitivity * dt;

            pitchRad = Mathf.Clamp(pitchRad, minPitchRad, maxPitchRad);

            // Scroll
            distance -= Input.GetAxis("Scroll") * scrollSensitivity * dt;
            distance -= Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

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
