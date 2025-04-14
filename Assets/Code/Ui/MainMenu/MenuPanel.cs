using UnityEngine;

namespace Ui.MainMenu
{
    public class MenuPanel : MonoBehaviour
    {
        public bool IsOpened => gameObject.activeSelf;

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