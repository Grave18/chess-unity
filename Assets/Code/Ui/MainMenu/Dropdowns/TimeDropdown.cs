using System.Collections.Generic;
using Ui.Common;
using UnityEngine;

namespace Ui.MainMenu.Dropdowns
{
    public class TimeDropdown : DropdownBase
    {
        [Header("References")]
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        [Header("Settings")]
        [SerializeField] private List<string> timesInMinutes = new()
        {
            "10", "15", "20", "25", "30", "35", "40", "45", "50", "55", "60",
        };

        protected override List<string> AddOptionsToDropdown()
        {
            return timesInMinutes;
        }

        protected override int SetCurrentOptionInDropdown(List<string> options)
        {
            string optionText = gameSettingsContainer.GetTime();
            int index = options.IndexOf(optionText);

            return index;
        }

        protected override void ApplyOption(string optionText, int index)
        {
            gameSettingsContainer.SetTime(optionText);
        }
    }
}