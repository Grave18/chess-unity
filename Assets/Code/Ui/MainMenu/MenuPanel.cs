using UnityEngine;

namespace Ui.MainMenu
{
    public class MenuPanel : MonoBehaviour
    {
        [SerializeField] private PanelManager panelManager;

        public bool IsOpened => gameObject.activeSelf;

        public void Show()
        {
            panelManager.SetCurrentPanel(this);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}