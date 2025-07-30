using UnityEngine;

namespace ChessGame
{
    public static class GInput
    {
        public static bool IsEnabled { get; set; } = true;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ClearStatics()
        {
            IsEnabled = true;
        }

        public static bool Lmb()
        {
            if (!IsEnabled) return false;
            return IsEnabled && Input.GetButton("Fire1");
        }

        public static bool Rmb()
        {
            if (!IsEnabled) return false;
            return  Input.GetButton("Fire2");
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
            if (!IsEnabled) return 0f;
            return Input.GetAxis("Mouse X") + Input.GetAxis("Horizontal");
        }

        public static float Vertical()
        {
            if (!IsEnabled) return 0f;
            return Input.GetAxis("Mouse Y") + Input.GetAxis("Vertical");
        }
    }
}