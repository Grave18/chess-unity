using LobbyManagement;
using PurrLobby;
using TMPro;
using UnityEngine;

namespace UnityUi.Menu.Lobby
{
    public class LobbyEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text lobbyNameText;
        [SerializeField] private TMP_Text playersText;

        private LobbyManagement.Lobby _room;
        private LobbyManager _lobbyManager;
        private UnityEngine.UI.Button _button;

        private void Awake()
        {
            _button = GetComponent<UnityEngine.UI.Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        public void Init(LobbyManagement.Lobby room, LobbyManager lobbyManager)
        {
            lobbyNameText.text = room.Name.Length > 0 ? room.Name : room.LobbyId;
            playersText.text = $"{room.Members.Count}/{room.MaxPlayers}";
            _room = room;
            _lobbyManager = lobbyManager;
        }

        private void OnClick()
        {
            _lobbyManager.JoinLobby(_room.LobbyId);
        }
    }
}
