using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Chess3D.Runtime.Menu.LobbyManagement;
using MvvmTool;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer.Unity;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    [INotifyPropertyChanged]
    public partial class LobbyViewModel : IViewModel, IDisposable, ITickable
    {
        // TODO: Move to config
        private const float UpdateIntervalSec = 3f;

        public ObservableCollection<FriendItem> Friends { get; set; } = new();
        public ObservableCollection<LobbyUser> LobbyUsers { get; set; } = new();

        [ObservableProperty] private LobbyManager.FriendFilter _friendFilter = LobbyManager.FriendFilter.Online;

        private float _lastUpdateTime;

        private readonly LobbyManager _lobbyManager;

        public LobbyViewModel(LobbyManager lobbyManager)
        {
            _lobbyManager = lobbyManager;

            _lobbyManager.OnFriendListPulled.AddListener(PopulateFriends);
            _lobbyManager.OnRoomLeft.AddListener(OnLobbyLeave);
            _lobbyManager.OnRoomUpdated.AddListener(LobbyDataUpdate);
        }

        public void Dispose()
        {
            if (_lobbyManager is null)
            {
                return;
            }

            _lobbyManager.OnFriendListPulled.RemoveListener(PopulateFriends);
            _lobbyManager.OnRoomLeft.RemoveListener(OnLobbyLeave);
            _lobbyManager.OnRoomUpdated.RemoveListener(LobbyDataUpdate);
        }

        public void Tick()
        {
            if(IsNeedToPullFriends())
            {
                PullFriends();
            }
        }

        private bool IsNeedToPullFriends()
        {
            return _lobbyManager.IsInLobby && _lastUpdateTime + UpdateIntervalSec < Time.time;
        }

        private void PullFriends()
        {
            _lastUpdateTime = Time.time;
            _lobbyManager.PullFriends(FriendFilter);
        }

        private void PopulateFriends(List<FriendItem> newQuery)
        {
            InitFriends(newQuery);
            RemoveOfflineFriends(newQuery);
            AddNewFriends(newQuery);
        }

        private void InitFriends(List<FriendItem> newQuery)
        {
            foreach (FriendItem friend in newQuery)
            {
                friend.Construct(_lobbyManager);
            }
        }

        private void RemoveOfflineFriends(List<FriendItem> newQuery)
        {
            var offlineFriends = Friends.Except(newQuery).ToList();

            foreach (FriendItem offlineFriend in offlineFriends)
            {
                Friends.Remove(offlineFriend);
            }
        }

        private void AddNewFriends(List<FriendItem> newQuery)
        {
            foreach (FriendItem friend in newQuery)
            {
                if (Friends.Contains(friend))
                {
                    continue;
                }

                Friends.Add(friend);
            }
        }

        private void OnLobbyLeave()
        {
            LobbyUsers.Clear();
        }

        private void LobbyDataUpdate(Lobby lobby)
        {
            if (!lobby.IsValid)
            {
                return;
            }

            AddNewMembers(lobby);
            RemoveLeftMembers(lobby);
        }

        private void AddNewMembers(Lobby room)
        {
            foreach (LobbyUser member in room.Members)
            {
                if (LobbyUsers.Contains(member))
                {
                    continue;
                }

                LobbyUsers.Add(member);
            }
        }

        private void RemoveLeftMembers(Lobby room)
        {
            var leftLobbyUsers = LobbyUsers.Except(room.Members).ToArray();

            foreach (LobbyUser lobbyUser in leftLobbyUsers)
            {
                LobbyUsers.Remove(lobbyUser);
            }
        }

        [RelayCommand]
        private void Ready()
        {
            _lobbyManager.ToggleLocalReady().Forget();
        }

        [RelayCommand]
        private void ChangeFriendFilter()
        {
            CycleThroughFilters();
            PullFriends();
        }

        private void CycleThroughFilters()
        {
            if (FriendFilter == LobbyManager.FriendFilter.InThisGame)
            {
                FriendFilter = LobbyManager.FriendFilter.Online;
            }
            else if (FriendFilter == LobbyManager.FriendFilter.Online)
            {
                FriendFilter = LobbyManager.FriendFilter.All;
            }
            else if (FriendFilter == LobbyManager.FriendFilter.All)
            {
                FriendFilter = LobbyManager.FriendFilter.InThisGame;
            }
        }

        [RelayCommand]
        private void Back()
        {
            _lobbyManager.LeaveLobby();
        }
    }
}