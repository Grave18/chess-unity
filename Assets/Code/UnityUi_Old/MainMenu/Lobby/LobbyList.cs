using System.Collections.Generic;
using PurrLobby;
using UnityEngine;

namespace Ui.MainMenu.Lobby
{
    public class LobbyList : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private LobbyEntry lobbyEntryPrefab;
        [SerializeField] private Transform content;

        [Header("Settings")]
        [SerializeField] private float updateInterval = 5f;

        private bool _isActive;
        private float _lastSearchTime;

        private void OnEnable()
        {
            lobbyManager.OnRoomSearchResults.AddListener(Populate);
            EnableLobby();
        }

        private void OnDisable()
        {
            lobbyManager.OnRoomSearchResults.RemoveListener(Populate);
            DisableLobby();
        }

        private void EnableLobby()
        {
            lobbyManager.SearchLobbies();
            _lastSearchTime = Time.time;
            _isActive = true;
        }

        private void DisableLobby()
        {
            _isActive = false;
        }

        private void Populate(List<PurrLobby.Lobby> rooms)
        {
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }

            foreach (PurrLobby.Lobby room in rooms)
            {
                LobbyEntry entry = Instantiate(lobbyEntryPrefab, content);
                entry.Init(room, lobbyManager);
            }
        }

        private void Update()
        {
            if (!_isActive)
            {
                return;
            }

            if (_lastSearchTime + updateInterval < Time.time)
            {
                _lastSearchTime = Time.time;
                lobbyManager.SearchLobbies();
            }
        }
    }
}