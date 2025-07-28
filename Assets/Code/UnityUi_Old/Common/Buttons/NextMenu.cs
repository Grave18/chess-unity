using Ui.MainMenu;
using UnityEngine;

namespace Ui.Common.Buttons
{
    [RequireComponent(typeof(ButtonBase))]
    public class NextMenu : ButtonCallbackBase
    {
        [SerializeField] private MenuPanel nextPanel;

        protected override void OnClick()
        {
            nextPanel?.Show();
        }
    }
}