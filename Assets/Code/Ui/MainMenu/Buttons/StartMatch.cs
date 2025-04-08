using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ui.MainMenu.Buttons
{
    public class StartMatch : ButtonCallbackBase
    {
        [SerializeField] private string sceneName = "Game";
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        protected override void OnClick()
        {
            gameSettingsContainer.SetComputerGame();
            SceneManager.LoadSceneAsync(sceneName);
        }
    }
}