using UnityEngine;

namespace Ui.MainMenu.Buttons
{
    [RequireComponent(typeof(Button))]
    public class NextMenu : ButtonCallbackBase
    {
        [SerializeField] private MenuPanel nextPanel;

        private MenuPanel _thisMenuPanel;

        protected override void Awake()
        {
            base.Awake();
            _thisMenuPanel = GetComponentInParent<MenuPanel>();
        }

        protected override void OnClick()
        {
            if (nextPanel != null)
            {
                _thisMenuPanel?.Hide();
                nextPanel.Show();
            }
        }
    }
}