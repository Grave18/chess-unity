using ChessGame.Logic;
using GameAndScene;
using Ui.Common;
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
            PlayerPrefs.SetInt("IsServer", isServer ? 1 : 0);

            sceneLoader.LoadOnlineLocalhost();
        }
    }
}
