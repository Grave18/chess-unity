using LobbyManagement;
using UnityEngine;
using UnityUi.Common.Buttons;

namespace UnityUi.Menu.Buttons
{
    public class CreateLobby : ButtonCallbackBase
    {
        [SerializeField] private LobbyManager lobbyManager;

        protected override void OnClick()
        {
            lobbyManager.CreateRoom();
        }
    }
}