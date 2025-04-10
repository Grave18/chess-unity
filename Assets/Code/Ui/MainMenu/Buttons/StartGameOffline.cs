using Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ui.MainMenu.Buttons
{
    public class StartGameOffline : ButtonCallbackBase
    {
        [SerializeField] private SceneReference sceneReference;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        protected override void OnClick()
        {
            gameSettingsContainer.SetupGameOffline();
            SceneManager.LoadSceneAsync(sceneReference);
        }
    }
}