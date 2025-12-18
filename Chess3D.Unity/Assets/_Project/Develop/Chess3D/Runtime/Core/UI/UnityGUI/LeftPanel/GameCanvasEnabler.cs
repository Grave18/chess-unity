using UnityEngine;

namespace Chess3D.Runtime.Core.UI.UnityGUI.LeftPanel
{
    public class GameCanvasEnabler : MonoBehaviour
    {
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
