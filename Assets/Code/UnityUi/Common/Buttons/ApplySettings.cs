using Settings;
using UnityEngine;

namespace UnityUi.Common.Buttons
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