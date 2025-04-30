using PurrLobby;
using UnityEngine;

namespace Ui.MainMenu.Buttons
{
    [RequireComponent(typeof(Button))]
    public class LeaveLobby : ButtonCallbackBase
    {
        [SerializeField] private MenuPanel nextPanel;
        [SerializeField] private LobbyManager lobbyManager;

        private MenuPanel _thisMenuPanel;

        protected override void Awake()
        {
            base.Awake();
            _thisMenuPanel = GetComponentInParent<MenuPanel>();
        }

        protected override void OnClick()
        {
            lobbyManager.LeaveLobby();
            nextPanel?.Show();
        }
    }
}