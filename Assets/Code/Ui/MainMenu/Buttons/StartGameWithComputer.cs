using GameAndScene;
using Ui.Common;
using UnityEngine;

namespace Ui.MainMenu.Buttons
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