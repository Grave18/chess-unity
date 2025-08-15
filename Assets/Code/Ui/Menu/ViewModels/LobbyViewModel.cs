using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LobbyManagement;
using UnityEngine;

namespace Ui.Menu.ViewModels
{
    public partial class LobbyViewModel : MonoBehaviour
    {
        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private LobbyManager.FriendFilter filter = LobbyManager.FriendFilter.All;
        [SerializeField] private float updateIntervalSec = 3f;

        private float _lastUpdateTime;

        public ObservableCollection<FriendUser> Friends { get; set; } = new();
        public ObservableCollection<LobbyUser> LobbyUsers { get; set; } = new();

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

        private void PopulateFriends(List<FriendUser> newQuery)
        {
            RemoveOfflineFriends(newQuery);
            AddNewFriends(newQuery);
        }

        private void RemoveOfflineFriends(List<FriendUser> newQuery)
        {
            var offlineFriends = Friends.Except(newQuery);

            foreach (FriendUser offlineFriend in offlineFriends)
            {
                Friends.Remove(offlineFriend);
            }
        }

        private void AddNewFriends(List<FriendUser> newQuery)
        {
            foreach (FriendUser friend in newQuery)
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

            SetReadyFeedback(lobby);
            AddNewMembers(lobby);
            RemoveLeftMembers(lobby);
        }

        private void SetReadyFeedback(Lobby room)
        {
            // foreach (Transform child in content)
            // {
            //     if (!child.TryGetComponent(out MemberEntry member))
            //     {
            //         continue;
            //     }
            //
            //     LobbyUser matchingMember = room.Members.Find((LobbyUser x) => x.Id == member.MemberId);
            //     if (!string.IsNullOrEmpty(matchingMember.Id))
            //     {
            //         // Set ready color
            //         member.SetReady(matchingMember.IsReady);
            //     }
            // }
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
            var leftLobbyUsers = LobbyUsers.Except(room.Members);

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
    }
}