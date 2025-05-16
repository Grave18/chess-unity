using PurrLobby;
using Ui.Common;
using Ui.Common.Buttons;
using UnityEngine;

namespace Ui.MainMenu.Buttons
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