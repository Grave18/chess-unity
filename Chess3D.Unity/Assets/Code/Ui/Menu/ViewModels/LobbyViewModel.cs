using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LobbyManagement;
using MvvmTool;
using UnityEngine;

namespace Ui.Menu.ViewModels
{
    public partial class LobbyViewModel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LobbyManager lobbyManager;

        [Header("Settings")]
        [SerializeField] private LobbyManager.FriendFilter filter = LobbyManager.FriendFilter.All;
        [SerializeField] private float updateIntervalSec = 3f;

        private float _lastUpdateTime;

        public ObservableCollection<FriendItem> Friends { get; set; } = new();
        public ObservableCollection<LobbyUser> LobbyUsers { get; set; } = new();

        [RelayCommand]
        private void Ready()
        {
            lobbyManager.ToggleLocalReady();
        }

        private void OnEnable()
        {
            lobbyManager.OnFriendListPulled.AddListener(PopulateFriends);
            lobbyManager.OnRoomLeft.AddListener(OnLobbyLeave);
            lobbyManager.OnRoomUpdated.AddListener(LobbyDataUpdate);
            PullFriends();
        }

        private void OnDisable()
        {
            lobbyManager.OnFriendListPulled.RemoveListener(PopulateFriends);
            lobbyManager.OnRoomLeft.RemoveListener(OnLobbyLeave);
            lobbyManager.OnRoomUpdated.RemoveListener(LobbyDataUpdate);
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
                friend.Init(lobbyManager);
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

        private void Update()
        {
            if(IsNeedToPullFriends())
            {
                PullFriends();
            }
        }

        private bool IsNeedToPullFriends()
        {
            return lobbyManager.IsInLobby && _lastUpdateTime + updateIntervalSec < Time.time;
        }

        private void PullFriends()
        {
            _lastUpdateTime = Time.time;
            lobbyManager.PullFriends(filter);
        }

        [RelayCommand]
        private void Back()
        {
            Debug.Log("Leve Lobby");
            lobbyManager.LeaveLobby();
        }
    }
}