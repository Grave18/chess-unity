using SceneManagement;
using Settings;
using UnityEngine;
using UnityUi.Common.Buttons;

namespace UnityUi.Menu.Buttons
{
    public class StartGameWithComputer : ButtonCallbackBase
    {
        [SerializeField] private SceneLoader sceneReference;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        protected override void OnClick()
        {
            gameSettingsContainer.SetupGameWithComputer();
            sceneReference.LoadGame();
        }
    }
}