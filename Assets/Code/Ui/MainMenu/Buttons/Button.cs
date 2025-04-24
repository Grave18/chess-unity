using TMPro;
using Ui.MainMenu.Classes;
using UnityEngine;
using UnityEngine.UI;
using Utils;

#if UNITY_EDITOR
using EditorCools;
using UnityEditor;
#endif

namespace Ui.MainMenu.Buttons
{
    public class Button : MonoBehaviour
    {
        [SerializeField] private string text = "Default";
        [SerializeField] private ButtonClass buttonClass;

        private UnityEngine.UI.Button _button;
        private TMP_Text _text;
        private RectTransform _transform;

        protected virtual void Awake()
        {
            _button = GetComponentInChildren<UnityEngine.UI.Button>();
            _text = _button.GetComponentInChildren<TMP_Text>();
            _transform = _button.GetComponent<RectTransform>();
        }

        protected virtual void OnEnable()
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

#if UNITY_EDITOR
        // private void OnValidate()
        [Button(name: "Apply Class", space: 10)]
        private void ApplyClassInEditor()
        {
            Awake();
            RecordUndo();
            ApplyClass();
        }

        private void RecordUndo()
        {
            Undo.RecordObject(_text, "Apply Class");
            Undo.RecordObject(_button, "Apply Class");
            Undo.RecordObject(_transform, "Apply Class");
            PrefabUtility.RecordPrefabInstancePropertyModifications(_text);
            PrefabUtility.RecordPrefabInstancePropertyModifications(_button);
            PrefabUtility.RecordPrefabInstancePropertyModifications(_transform);
        }
#endif
    }
}