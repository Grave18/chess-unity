using PurrLobby;
using Ui.MainMenu.Buttons;
using UnityEngine;

namespace Ui.MainMenu.Lobby
{
    public class CreateLobbyButton : ButtonCallbackBase
    {
        [SerializeField] private LobbyManager lobbyManager;

        protected override void OnClick()
        {
            lobbyManager.CreateRoom();
        }
    }
}