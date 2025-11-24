using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Chess3D.Runtime.Menu.LobbyManagement;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using LobbyManagement;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer.Unity;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    public class FindGameViewModel : ILoadUnit, ITickable, IDisposable
    {
        private const float UpdateIntervalSec = 3f;

        private LobbyManager _lobbyManager;

        public ObservableCollection<LobbyUser> Lobbies { get; set; } = new();

        private bool _isActive;
        private float _lastSearchTime;

        public FindGameViewModel(LobbyManager lobbyManager)
        {
            _lobbyManager = lobbyManager;
            _lobbyManager.OnRoomSearchResults.AddListener(PopulateRoom);
        }

        public UniTask Load()
        {
            EnableLobby();
            return UniTask.CompletedTask;
        }

        private void EnableLobby()
        {
            _lobbyManager.SearchLobbies();
            _lastSearchTime = Time.time;
            _isActive = true;
        }

        public void Dispose()
        {
            _lobbyManager.OnRoomSearchResults.RemoveListener(PopulateRoom);
            DisableLobby();
        }

        private void DisableLobby()
        {
            _isActive = false;
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

        public void Tick()
        {
            if (!_isActive)
            {
                return;
            }

            if (_lastSearchTime + UpdateIntervalSec < Time.time)
            {
                _lastSearchTime = Time.time;
                _lobbyManager.SearchLobbies();
            }
        }
    }
}