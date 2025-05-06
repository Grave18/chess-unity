using Ui.Common;
using UnityEngine;

namespace Ui.MainMenu.Buttons
{
    [RequireComponent(typeof(ButtonBase))]
    public class ApplySettings : ButtonCallbackBase
    {
        [SerializeField] private GraphicsSettingsContainer graphicsSettingsContainer;

        protected override void OnClick()
        {
            graphicsSettingsContainer?.ApplySettings();
        }
    }
}