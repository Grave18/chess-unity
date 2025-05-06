using Ui.Common;
using UnityEngine;

namespace Ui.MainMenu.Buttons
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