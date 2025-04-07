using TMPro;
using Ui.MainMenu.Classes;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Ui.MainMenu.Buttons
{
    public class Button : MonoBehaviour
    {
        [SerializeField] private string text = "Default";
        [SerializeField] private ButtonClass buttonClass;

        private UnityEngine.UI.Button _button;
        private TMP_Text _text;
        private RectTransform _transform;

        private void Awake()
        {
            _button = GetComponentInChildren<UnityEngine.UI.Button>();
            _text = _button.GetComponentInChildren<TMP_Text>();
            _transform = _button.GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            ApplyClass();
        }

        private void ApplyClass()
        {
            _text.color = buttonClass.FgColor;
            _text.text = text;
            _text.fontSize = buttonClass.TextSize;

            if (buttonClass.Font != null)
            {
                _text.font = buttonClass.Font;
            }
            if (buttonClass.BackgroundImage != null)
            {
                _button.image.sprite = buttonClass.BackgroundImage;
            }

            ColorBlock colors = _button.colors;
            colors.normalColor = buttonClass.BgColor;
            colors.highlightedColor = buttonClass.BgHighlightColor;
            _button.colors = colors;

            _transform.SetAligned(buttonClass.Width, buttonClass.Height,
                buttonClass.HorizontalAlignment, buttonClass.VerticalAlignment);
        }
    }
}