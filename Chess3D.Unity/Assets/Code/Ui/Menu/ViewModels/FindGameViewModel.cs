using System.Collections.Generic;
using System.Collections.ObjectModel;
using LobbyManagement;
using UnityEngine;

namespace Ui.Menu.ViewModels
{
    public class FindGameViewModel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LobbyManager lobbyManager;

        [Header("Settings")]
        [SerializeField] private float updateIntervalSec = 3f;

        public ObservableCollection<LobbyUser> Lobbies { get; set; } = new();

        private bool _isActive;
        private float _lastSearchTime;

        private void OnEnable()
        {
            lobbyManager.OnRoomSearchResults.AddListener(PopulateRoom);
            EnableLobby();
        }

        private void OnDisable()
        {
            lobbyManager.OnRoomSearchResults.RemoveListener(PopulateRoom);
            DisableLobby();
        }

        private void PopulateRoom(List<Lobby> rooms)
        {
            Lobbies.Clear();

            foreach (Lobby room in rooms)
            {
                LobbyUser lobby = new()
                {
                    Id = room.LobbyId,
                    Name = room.Name,
                };

                Lobbies.Add(lobby);
            }
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

        private void Update()
        {
            if (!_isActive)
            {
                return;
            }

            if (_lastSearchTime + updateIntervalSec < Time.time)
            {
                _lastSearchTime = Time.time;
                lobbyManager.SearchLobbies();
            }
        }
    }
}