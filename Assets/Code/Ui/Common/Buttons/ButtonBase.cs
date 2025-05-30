using EditorCools;
using TMPro;
using Ui.Common.Classes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;

namespace Ui.Common.Buttons
{
    public class ButtonBase : MonoBehaviour
    {
        [SerializeField] private string text = "Default";
        [SerializeField] private ButtonClass buttonClass;

        private Button _button;
        private TMP_Text _text;
        private RectTransform _transform;

        public string Text
        {
            get => _text.text;
            set => _text.text = value;
        }

        public bool Interactable
        {
            get => _button.interactable;
            set
            {
                _button.interactable = value;
                _text.color = value ? buttonClass.TextColor : buttonClass.TextDisabledColor;
            }
        }

        public event UnityAction OnClick
        {
            add => _button.onClick.AddListener(value);
            remove => _button.onClick.RemoveListener(value);
        }

        private void Awake()
        {
            _button = GetComponentInChildren<Button>();
            _text = _button.GetComponentInChildren<TMP_Text>();
            _transform = _button.GetComponent<RectTransform>();

            Assert.IsNotNull(_button, "Button not found");
            Assert.IsNotNull(_text, "Text not found");
            Assert.IsNotNull(_transform, "Transform not found");
        }

        private void OnEnable()
        {
            ApplyClass();
        }

        private void ApplyClass()
        {
            _text.color = _button.interactable ? buttonClass.TextColor : buttonClass.TextDisabledColor;
            _text.text = text;
            _text.fontSize = buttonClass.TextSize;

            if (buttonClass.Font)
            {
                _text.font = buttonClass.Font;
            }

            if (buttonClass.BackgroundImage)
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

#if UNITY_EDITOR

        [Button(name: "Apply Class", space: 10)]
        private void ApplyClassInEditor()
        {
            Awake();
            RecordUndo();
            ApplyClass();
        }

        private void RecordUndo()
        {
            Undo.RecordObject(_text, "Apply Button Class");
            Undo.RecordObject(_button, "Apply Button Class");
            Undo.RecordObject(_transform, "Apply Button Class");
            PrefabUtility.RecordPrefabInstancePropertyModifications(_text);
            PrefabUtility.RecordPrefabInstancePropertyModifications(_button);
            PrefabUtility.RecordPrefabInstancePropertyModifications(_transform);
        }

#endif
    }
}