using Chess3D.Runtime.Menu.UI.UnityGUI;
using IngameDebugConsole;
using UnityEngine;

namespace Chess3D.Runtime.Bootstrap
{
    public sealed class AppDebugView : MonoBehaviour
    {
        [SerializeField] private BuildVersionDisplayer buildVersionDisplayer;
        [SerializeField] private DebugLogManager debugLogManager;

        private void Awake()
        {
#if GRAVEBOX_PROD
            Destroy(gameObject);
#else
            DontDestroyOnLoad(gameObject);
#endif
        }
    }
}