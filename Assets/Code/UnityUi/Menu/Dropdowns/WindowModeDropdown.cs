using System.Collections.Generic;
using Settings;
using UnityEngine;
using UnityUi.Common;

namespace UnityUi.Menu.Dropdowns
{
    public class WindowModeDropdown : DropdownBase
    {
        [SerializeField] protected GraphicsSettingsContainer graphicsSettingsContainer;

        protected override List<string> AddOptionsToDropdown()
        {
            return new List<string>
            {
                "FullScreen",
                "Borderless",
                "Maximized Window",
                "Windowed",
            };
        }

        protected override int SetCurrentOptionInDropdown(List<string> options)
        {
            return graphicsSettingsContainer.GetFullScreenMode();
        }

        protected override void ApplyOption(string optionText, int index)
        {
            graphicsSettingsContainer.SetFullScreenMode(index);
        }
    }
}