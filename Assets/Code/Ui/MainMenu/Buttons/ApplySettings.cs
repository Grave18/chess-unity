using UnityEngine;

namespace Ui.MainMenu.Buttons
{
    [RequireComponent(typeof(Button))]
    public class ApplySettings : ButtonCallbackBase
    {
        [SerializeField] private GraphicsSettingsContainer graphicsSettingsContainer;

        protected override void OnClick()
        {
            graphicsSettingsContainer?.ApplySettings();
        }
    }
}