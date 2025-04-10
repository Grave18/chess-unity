using TMPro;
using UnityEngine;

namespace Ui.MainMenu.TextBoxes
{
    public abstract class TextBoxBase : MonoBehaviour
    {
        private TMP_InputField _textBox;

        protected virtual void Awake()
        {
            _textBox = GetComponentInChildren<TMP_InputField>();
        }

        protected virtual void OnEnable()
        {
            _textBox.onEndEdit.AddListener(OnEndEdit);
        }

        protected virtual void OnDisable()
        {
            _textBox.onEndEdit.RemoveListener(OnEndEdit);
        }

        protected void SetText(string text)
        {
            _textBox.text = text;
        }

        protected abstract void OnEndEdit(string value);
    }
}