using System.Collections.Generic;
using System.Linq;
using GameAndScene;
using Ui.Common;
using UnityEngine;

namespace Ui.MainMenu.Dropdowns
{
    public class QualityDropdown : DropdownBase
    {
        [SerializeField] protected GraphicsSettingsContainer graphicsSettingsContainer;

        protected override int SetCurrentOptionInDropdown(List<string> options)
        {
            return graphicsSettingsContainer.GetQualityIndex();
        }

        protected override List<string> AddOptionsToDropdown()
        {
            return QualitySettings.names.ToList();
        }

        protected override void ApplyOption(string optionText, int index)
        {
            graphicsSettingsContainer.SetQuality(index);
        }
    }
}