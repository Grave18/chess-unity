using PurrLobby;
using Ui.Common;
using Ui.Common.Buttons;
using Ui.MainMenu.Buttons;
using UnityEngine;

namespace Ui.MainMenu.Lobby
{
    public class BrowseButton : ButtonCallbackBase
    {
        [SerializeField] private LobbyManager lobbyManager;

        protected override void OnClick()
        {
            lobbyManager.CreateRoom();
        }
    }
}