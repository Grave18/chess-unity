using UnityEngine;

namespace Chess3D.Runtime.Core.ChessBoard
{
    public class KnightRotator : MonoBehaviour
    {
        [Tooltip("Angle in degrees toward center of the board")]
        [Range(-90, 90)]
        [SerializeField] private float angle;

        private void Start()
        {
            SetAngle();
        }

        private void SetAngle()
        {
            float currentAngle = transform.position.x > 0 ? angle : -angle;

            transform.Rotate(Vector3.up, currentAngle);
        }
    }
}