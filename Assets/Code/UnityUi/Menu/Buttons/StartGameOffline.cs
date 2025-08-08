using SceneManagement;
using Settings;
using UnityEngine;
using UnityUi.Common.Buttons;

namespace UnityUi.Menu.Buttons
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