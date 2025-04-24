using PurrLobby;
using Ui.MainMenu.Buttons;
using UnityEngine;

namespace Ui.MainMenu.Lobby
{
    public class Ready : ButtonCallbackBase
    {
        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        private bool _isReady;

        protected override void OnClick()
        {
            gameSettingsContainer.SetupGameOnline();
            lobbyManager.ToggleLocalReady();
        }
    }
}