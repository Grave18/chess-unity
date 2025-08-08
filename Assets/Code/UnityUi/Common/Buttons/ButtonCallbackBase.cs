using UnityEngine;

namespace UnityUi.Common.Buttons
{
    public abstract class ButtonCallbackBase : MonoBehaviour
    {
        private UnityEngine.UI.Button button;

        protected virtual void Awake()
        {
            button = GetComponentInChildren<UnityEngine.UI.Button>();
        }

        protected virtual void OnEnable()
        {
            button?.onClick.AddListener(OnClick);
        }

        protected virtual void OnDisable()
        {
            button?.onClick.RemoveListener(OnClick);
        }

        protected abstract void OnClick();
    }
}