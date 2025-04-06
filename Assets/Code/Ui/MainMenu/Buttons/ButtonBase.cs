using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Ui.MainMenu.Buttons
{
    [RequireComponent(typeof(Button))]
    public abstract class ButtonBase : MonoBehaviour
    {
        [SerializeField] private string @class = "Button";

        [SerializeField] private string text = "Button";
        [SerializeField] private int textSize = 30;
        [SerializeField] private Font font;
        [SerializeField] private Color fgColor = Color.white;
        [SerializeField] private Color bgColor = Color.gray;

        private Button _button;

        protected virtual void Awake()
        {
            _button = GetComponent<Button>();

            // var textComponent = _button.GetComponentInChildren<Text>();
            // textComponent.fontSize = textSize;
            // textComponent.font = font;
            // textComponent.color = fgColor;
            // ColorBlock colors = _button.colors;
            // colors.normalColor = bgColor;
            // _button.colors = colors;
        }

        protected virtual void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        protected virtual void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        protected abstract void OnClick();
    }
}