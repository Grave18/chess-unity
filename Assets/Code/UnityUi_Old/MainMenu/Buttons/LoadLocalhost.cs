using Logic;
using SceneManagement;
using Settings;
using Ui.Common.Buttons;
using UnityEngine;

namespace Ui.MainMenu.Buttons
{
    public class LoadLocalhost : ButtonCallbackBase
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        [SerializeField] private bool isServer;

        protected override void OnClick()
        {

            gameSettingsContainer.SetupGameOnline(isServer ? PieceColor.White : PieceColor.Black);
            GameSettingsContainer.IsLocalhostServer = isServer;

            sceneLoader.LoadOnlineLocalhost();
        }
    }
}
