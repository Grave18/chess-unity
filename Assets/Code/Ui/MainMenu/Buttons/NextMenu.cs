using UnityEngine;

namespace Ui.MainMenu.Buttons
{
    [RequireComponent(typeof(Button))]
    public class NextMenu : ButtonCallbackBase
    {
        [SerializeField] private MenuPanel nextPanel;

        protected override void OnClick()
        {
            nextPanel?.Show();
        }
    }
}