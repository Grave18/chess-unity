using UnityEngine;

namespace Ui.MainMenu
{
    public class PanelManagerBase : MonoBehaviour
    {
        public MenuPanel CurrentPanel { get; private set; }

        protected virtual void Awake()
        {
            FindOpenedPanel();
        }

        private void FindOpenedPanel()
        {
            MenuPanel[] panels = FindObjectsByType<MenuPanel>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            foreach (MenuPanel panel in panels)
            {
                SetCurrentPanel(panel);
            }
        }

        public void SetCurrentPanel(MenuPanel panel)
        {
            CurrentPanel?.Hide();
            CurrentPanel = panel;
        }
    }
}