using UnityEngine;

namespace Ui.MainMenu.Buttons
{
    public class ApplySettingsButton : ButtonBase
    {
        [SerializeField] private GraphicsSettingsContainer graphicsSettingsContainer;

        protected override void OnClick()
        {
            graphicsSettingsContainer?.ApplySettings();
        }
    }
}