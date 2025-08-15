using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LobbyManagement;
using MvvmTool;
using UnityEngine;

namespace Ui.Menu.ViewModels
{
    [INotifyPropertyChanged]
    public partial class LobbyViewModel : MonoBehaviour
    {
        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private LobbyManager.FriendFilter filter = LobbyManager.FriendFilter.All;
        [SerializeField] private float updateIntervalSec = 3f;

        private float _lastUpdateTime;

        public ObservableCollection<FriendUser> Friends { get; set; } = new();

        private void OnEnable()
        {
            lobbyManager.OnFriendListPulled.AddListener(PopulateFriends);
            PullFriends();
        }

        private void OnDisable()
        {
            lobbyManager.OnFriendListPulled.RemoveListener(PopulateFriends);
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