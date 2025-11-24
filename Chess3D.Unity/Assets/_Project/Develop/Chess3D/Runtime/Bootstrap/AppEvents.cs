using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Bootstrap
{
    [Preserve]
    public class AppEvents : MonoBehaviour
    {
        public event Action<bool> OnPause;

        private void OnApplicationPause(bool pause)
        {
            OnPause?.Invoke(pause);
        }
    }
}