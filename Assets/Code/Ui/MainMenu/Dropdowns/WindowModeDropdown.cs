using System.Collections.Generic;
using Ui.Common;
using UnityEngine;

namespace Ui.MainMenu.Dropdowns
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