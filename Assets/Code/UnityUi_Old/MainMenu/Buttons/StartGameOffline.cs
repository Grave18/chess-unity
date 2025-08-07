using SceneManagement;
using Settings;
using Ui.Common.Buttons;
using UnityEngine;

namespace Ui.MainMenu.Buttons
{
    public class StartGameOffline : ButtonCallbackBase
    {
        [Header("References")]
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        protected override void OnClick()
        {
            gameSettingsContainer.SetupGameOffline();
            sceneLoader.LoadGame();
        }
    }
}