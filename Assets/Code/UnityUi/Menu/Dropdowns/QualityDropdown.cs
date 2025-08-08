using System.Collections.Generic;
using System.Linq;
using Settings;
using UnityEngine;
using UnityUi.Common;

namespace UnityUi.Menu.Dropdowns
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