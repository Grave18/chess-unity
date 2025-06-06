using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Ui.Common
{
    public abstract class DropdownBase : MonoBehaviour
    {
        private TMP_Dropdown _dropdown;
        private List<string> _options = new();

        protected void Awake()
        {
            _dropdown = GetComponentInChildren<TMP_Dropdown>();
            Assert.IsNotNull(_dropdown, $"{nameof(DropdownBase)}: Dropdown not found");

            _options = AddOptionsToDropdown();

            foreach (string option in _options)
            {
                _dropdown.options.Add(new TMP_Dropdown.OptionData(option));
            }
        }

        protected abstract List<string> AddOptionsToDropdown();

        protected virtual void OnEnable()
        {
            _dropdown.value = SetCurrentOptionInDropdown(_options);
            _dropdown.onValueChanged.AddListener(SetValue);
        }

        protected abstract int SetCurrentOptionInDropdown(List<string> options);

        protected virtual void OnDisable()
        {
            _dropdown.onValueChanged.RemoveListener(SetValue);
        }

        private void SetValue(int index)
        {
            string optionText = _dropdown.options[index].text;

            ApplyOption(optionText, index);
        }

        protected abstract void ApplyOption(string optionText, int index);
    }
}