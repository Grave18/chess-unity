using UnityEngine;

namespace Ui.MainMenu.Buttons
{
    public class ExitButton : ButtonBase
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