using Ui.Common;
using UnityEngine;

namespace Ui.MainMenu.Buttons
{
    [RequireComponent(typeof(ButtonBase))]
    public class Exit : ButtonCallbackBase
    {
        protected override void OnClick()
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}