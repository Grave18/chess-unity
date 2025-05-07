using ChessGame.Logic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;

namespace MainCamera
{
    [RequireComponent(typeof(CinemachineFreeLook))]
    public class CameraControllerCinemachine : MonoBehaviour
    {
        [Header("New Settings")]
        [SerializeField] private float cameraSpeedX = 200f;
        [SerializeField] private float cameraSpeedY = 2f;

        private CinemachineFreeLook _cameraFree;
        private bool _isInitialized;

        private void Awake()
        {
            _cameraFree = GetComponent<CinemachineFreeLook>();
            Assert.IsNotNull(_cameraFree);
        }

        public void Init(PieceColor color)
        {
            if (color == PieceColor.White)
            {

            }
            else if (color == PieceColor.Black)
            {

            }

            _isInitialized = true;
        }

        private void Update()
        {
            if (!_isInitialized)
            {
                return;
            }

            CalculateOrbit();
        }

        private void CalculateOrbit()
        {
            if (Input.GetButton("Fire2"))
            {
                _cameraFree.m_XAxis.m_InputAxisName = "Mouse X";
            }
            else
            {
                _cameraFree.m_XAxis.m_InputAxisName = "";
                _cameraFree.m_XAxis.m_InputAxisValue = 0f;
            }
        }
    }
}
