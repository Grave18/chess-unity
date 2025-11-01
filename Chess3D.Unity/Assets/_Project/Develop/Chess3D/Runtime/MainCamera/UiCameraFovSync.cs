using UnityEngine;
using UnityEngine.Assertions;

namespace Chess3D.Runtime.MainCamera
{
    public class UiCameraFovSync : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        private Camera uiCamera;

        private void Awake()
        {
            uiCamera = GetComponent<Camera>();
            Assert.IsNotNull(uiCamera);
        }

        private void LateUpdate()
        {
            uiCamera.fieldOfView = mainCamera.fieldOfView;
        }
    }
}