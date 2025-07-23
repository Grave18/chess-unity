using EditorCools;
using TMPro;
using Ui.Common.Classes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;
using UtilsCommon;

namespace Ui.Common.Buttons
{
    [DefaultExecutionOrder(-1)] // Because Awake must be called early
    public class ButtonBase : MonoBehaviour
    {
        [SerializeField] private string text = "Default";
        [SerializeField] private ButtonClass buttonClass;

        private Button _button;
        private TMP_Text _text;
        private RectTransform _transform;

        private bool _isInitialized;

        public string Text
        {
            get
            {
                if (!_isInitialized) Awake();
                return _text.text;
            }
            set
            {
                if (!_isInitialized) Awake();
                _text.text = value;
            }
        }

        public bool Interactable
        {
            get
            {
                if (!_isInitialized) Awake();
                return _button.interactable;
            }
            set
            {
                if (!_isInitialized) Awake();
                _button.interactable = value;
                _text.color = value ? buttonClass.TextColor : buttonClass.TextDisabledColor;
            }
        }

        public event UnityAction OnClick
        {
            add
            {
                if (!_isInitialized) Awake();
                _button.onClick.AddListener(value);
            }
            remove
            {
                if (!_isInitialized) Awake();
                _button.onClick.RemoveListener(value);
            }
        }

        private void Awake()
        {
            if(_isInitialized) return;
            _button = GetComponentInChildren<Button>();
            _text = _button.GetComponentInChildren<TMP_Text>();
            _transform = _button.GetComponent<RectTransform>();

            Assert.IsNotNull(_button, "Button not found");
            Assert.IsNotNull(_text, "Text not found");
            Assert.IsNotNull(_transform, "Transform not found");

            _isInitialized = true;
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