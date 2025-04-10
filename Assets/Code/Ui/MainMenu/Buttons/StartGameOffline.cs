using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ui.MainMenu.Buttons
{
    public class StartGameOffline : ButtonCallbackBase
    {
        [SerializeField] private string sceneName = "Game";
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        protected override void OnClick()
        {
            gameSettingsContainer.SetupGameOffline();
            SceneManager.LoadSceneAsync(sceneName);
        }
    }
}