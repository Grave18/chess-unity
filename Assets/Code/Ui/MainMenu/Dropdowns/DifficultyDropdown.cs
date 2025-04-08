using System;
using System.Collections.Generic;
using System.Linq;
using Ai;
using UnityEngine;

namespace Ui.MainMenu.Dropdowns
{
    public class DifficultyDropdown : DropdownBase
    {
        [SerializeField] private GameSettingsContainer gameSettings;

        protected override List<string> AddOptionsToDropdown()
        {
            return Enum.GetNames(typeof(ComputerSkillLevel)).ToList();
        }

        protected override int SetCurrentOptionInDropdown(List<string> options)
        {
            string option = gameSettings.GetDifficulty();
            int index = options.IndexOf(option);

            return index;
        }

        protected override void ApplyOption(string optionText, int index)
        {
            gameSettings.SetDifficulty(optionText);
        }
    }
}