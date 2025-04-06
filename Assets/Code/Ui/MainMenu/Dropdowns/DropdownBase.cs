using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Ui.MainMenu.Dropdowns
{
    public abstract class DropdownBase : MonoBehaviour
    {
        [SerializeField] protected GraphicsSettingsContainer graphicsSettingsContainer;

        private TMP_Dropdown _dropdown;
        private List<string> _options = new();

        protected void Awake()
        {
            _dropdown = GetComponentInChildren<TMP_Dropdown>();
            _options = AddOptionsToDropdown();

            foreach (string option in _options)
            {
                _dropdown.options.Add(new TMP_Dropdown.OptionData(option));
            }
        }

        protected abstract List<string> AddOptionsToDropdown();

        protected void OnEnable()
        {
            _dropdown.onValueChanged.AddListener(SetValue);

            _dropdown.value = SetCurrentOptionInDropdown(_options);
        }

        protected abstract int SetCurrentOptionInDropdown(List<string> options);

        protected void OnDisable()
        {
            _dropdown.onValueChanged.RemoveListener(SetValue);
        }

        private void SetValue(int value)
        {
            string optionText = _dropdown.options[value].text;

            ApplyOption(optionText, value);
        }

        protected abstract void ApplyOption(string optionText, int index);
    }
}