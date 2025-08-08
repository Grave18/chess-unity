using UnityEngine;
using UnityUi.Menu;

namespace UnityUi.Common.Buttons
{
    [RequireComponent(typeof(ButtonBase))]
    public class NextMenu : ButtonCallbackBase
    {
        [SerializeField] private MenuPanel nextPanel;

        protected override void OnClick()
        {
            nextPanel?.Show();
        }
    }
}