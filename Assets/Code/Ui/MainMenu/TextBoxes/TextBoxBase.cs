using TMPro;
using UnityEngine;

namespace Ui.MainMenu.TextBoxes
{
    public abstract class TextBoxBase : MonoBehaviour
    {
        protected TMP_InputField TextBox;

        protected virtual void Awake()
        {
            TextBox = GetComponentInChildren<TMP_InputField>();
        }

        protected virtual void OnEnable()
        {
            TextBox.onEndEdit.AddListener(OnEndEdit);
        }

        protected virtual void OnDisable()
        {
            TextBox.onEndEdit.RemoveListener(OnEndEdit);
        }

        protected abstract void OnEndEdit(string value);
    }
}