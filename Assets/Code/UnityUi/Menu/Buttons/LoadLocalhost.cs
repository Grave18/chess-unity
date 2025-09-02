using Logic;
using SceneManagement;
using Settings;
using UnityEngine;
using UnityUi.Common.Buttons;

namespace UnityUi.Menu.Buttons
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

            sceneLoader.LoadOnline();
        }
    }
}
