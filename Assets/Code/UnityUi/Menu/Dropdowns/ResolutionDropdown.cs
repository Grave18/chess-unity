using System.Collections.Generic;
using System.Linq;
using Settings;
using UnityEngine;
using UnityUi.Common;

namespace UnityUi.Menu.Dropdowns
{
    public class ResolutionDropdown : DropdownBase
    {
        [SerializeField] protected GraphicsSettingsContainer graphicsSettingsContainer;

        protected override List<string> AddOptionsToDropdown()
        {
            IEnumerable<Resolution> resolutions = Screen.resolutions.Reverse();
            var options = new HashSet<string>();

            foreach (Resolution res in resolutions)
            {
                string resolution = $"{res.width}x{res.height}";
                options.Add(resolution);
            }

            return options.ToList();
        }

        protected override int SetCurrentOptionInDropdown(List<string> options)
        {
            Resolution selectedResolution = graphicsSettingsContainer.GetResolution();
            string resolution = $"{selectedResolution.width}x{selectedResolution.height}";

            if(options.Contains(resolution))
            {
                int index = options.IndexOf(resolution);
                return index;
            }

            return 0;
        }

        protected override void ApplyOption(string optionText, int index)
        {
            string[] resolutionSplit = optionText.Split('x');
            bool isWidth = int.TryParse(resolutionSplit[0], out int width);
            bool isHeight = int.TryParse(resolutionSplit[1], out int height);

            if (isWidth && isHeight)
            {
                graphicsSettingsContainer.SetResolution(new Resolution{width = width, height = height});
            }
            else
            {
                graphicsSettingsContainer.SetResolution(new Resolution{width = 800, height = 600});
            }
        }
    }
}