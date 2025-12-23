using UnityEngine;

namespace Chess3D.Runtime.Core.InputManagement
{
    public static class InputController
    {
        public static bool IsEnabled { get; set; } = true;
        public static bool IsOrbitEnabled { get; set; } = true;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ClearStatics()
        {
            IsEnabled = true;
            IsOrbitEnabled = true;
        }

        public static bool GetLmb()
        {
            return IsEnabled && Input.GetButton("Fire1");
        }

        public static bool GetLmbDown()
        {
            return IsEnabled && Input.GetButtonDown("Fire1");
        }

        public static bool GetLmbUp()
        {
            return IsEnabled && Input.GetButtonUp("Fire1");
        }

        public static bool GetRmb()
        {
            return IsEnabled && Input.GetButton("Fire2");
        }

        public static bool GetRmbDown()
        {
            return IsEnabled && Input.GetButtonDown("Fire2");
        }

        public static bool GetRmbUp()
        {
            return IsEnabled && Input.GetButtonUp("Fire2");
        }

        public static Vector3 MousePosition()
        {
            if (!IsEnabled) return Vector3.zero;
            return Input.mousePosition;
        }

        public static float Scroll()
        {
            if (!IsEnabled) return 0f;
            return Input.GetAxis("Mouse ScrollWheel") + Input.GetAxis("Scroll") * 2f;
        }

        public static float Horizontal()
        {
            if (!IsEnabled || !IsOrbitEnabled) return 0f;
            return Input.GetAxis("Mouse X") + Input.GetAxis("Horizontal");
        }

        public static float Vertical()
        {
            if (!IsEnabled || !IsOrbitEnabled) return 0f;
            return Input.GetAxis("Mouse Y") + Input.GetAxis("Vertical");
        }
    }
}