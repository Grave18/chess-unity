using LobbyManagement;
using PurrLobby;
using UnityEngine;
using UnityUi.Common.Buttons;

namespace UnityUi.Menu.Lobby
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