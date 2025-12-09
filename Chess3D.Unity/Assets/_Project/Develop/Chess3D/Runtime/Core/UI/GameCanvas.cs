using UnityEngine;

namespace Chess3D.Runtime.Core.UI
{
    public class GameCanvas : MonoBehaviour
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
