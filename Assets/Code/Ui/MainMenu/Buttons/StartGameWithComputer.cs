using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ui.MainMenu.Buttons
{
    public class StartGameWithComputer : ButtonCallbackBase
    {
        [SerializeField] private string sceneName = "Game";
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        protected override void OnClick()
        {
            gameSettingsContainer.SetupGameWithComputer();
            SceneManager.LoadSceneAsync(sceneName);
        }
    }
}