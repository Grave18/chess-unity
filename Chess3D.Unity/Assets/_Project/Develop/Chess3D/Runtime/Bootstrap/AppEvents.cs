using UniRx;
using UnityEngine;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Bootstrap
{
    [Preserve]
    public class AppEvents : MonoBehaviour
    {
        public Subject<bool> OnPause = new();

        private void OnApplicationPause(bool pause)
        {
            OnPause.OnNext(pause);
        }
    }
}