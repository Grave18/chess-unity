using UnityEngine;

namespace Chess3D.Runtime.Online
{
    public class WaitCanvas : MonoBehaviour
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