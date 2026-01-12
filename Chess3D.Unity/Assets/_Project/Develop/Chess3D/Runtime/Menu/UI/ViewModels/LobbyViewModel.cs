using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Chess3D.Runtime.Menu.LobbyManagement;
using Chess3D.Runtime.Online;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using MvvmTool;
using UniRx;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    [INotifyPropertyChanged]
    public sealed partial class LobbyViewModel : IViewModel, IDisposable
    {
        public ObservableCollection<FriendItem> Friends { get; set; } = new();
        public ObservableCollection<LobbyUser> LobbyUsers { get; set; } = new();

        [ObservableProperty] private LobbyManager.FriendFilter _friendFilter = LobbyManager.FriendFilter.Online;

        private readonly LobbyManager _lobbyManager;
        private readonly ConfigContainer _configContainer;
        private readonly SceneManager _sceneManager;

        private readonly CompositeDisposable _disposables = new();

        public LobbyViewModel(LobbyManager lobbyManager, ConfigContainer configContainer, SceneManager sceneManager)
        {
            _lobbyManager = lobbyManager;
            _configContainer = configContainer;
            _sceneManager = sceneManager;

            _lobbyManager.OnRoomJoined
                .Subscribe(_ => _lobbyManager.PullFriends(FriendFilter))
                .AddTo(_disposables);

            _lobbyManager.OnRoomLeft
                .Subscribe(_ => LobbyUsers.Clear())
                .AddTo(_disposables);

            _lobbyManager.OnFriendListPulled
                .Subscribe(PopulateFriends)
                .AddTo(_disposables);

            _lobbyManager.OnRoomUpdated
                .Subscribe(LobbyDataUpdate)
                .AddTo(_disposables);

            lobbyManager.OnAllReady
                .Subscribe(_ => LoadOnline().Forget())
                .AddTo(_disposables);

            Observable.Interval(TimeSpan.FromSeconds(configContainer.LobbyConfig.UpdateIntervalSec))
                .Where(_ => _lobbyManager.IsInLobby)
                .Subscribe(_ => _lobbyManager.PullFriends(FriendFilter))
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
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

        private async UniTaskVoid LoadOnline()
        {
            _configContainer.OnlineConfig.ConnectionType = ConnectionType.Online;
            _configContainer.OnlineConfig.IsLocal = false;

            if (_lobbyManager.CurrentLobby.IsOwner)
            {
                _configContainer.OnlineConfig.AuthorityType = AuthorityType.Host;
            }
            else
            {
                _configContainer.OnlineConfig.AuthorityType = AuthorityType.Client;
                await UniTask.WaitForSeconds(1f, ignoreTimeScale: true);
            }

            _sceneManager.LoadScene(RuntimeConstants.Scenes.Online, true).Forget();
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
            _lobbyManager.PullFriends(FriendFilter);
        }

        private void CycleThroughFilters()
        {
            FriendFilter = FriendFilter switch
            {
                LobbyManager.FriendFilter.Online => LobbyManager.FriendFilter.InThisGame,
                LobbyManager.FriendFilter.InThisGame => LobbyManager.FriendFilter.All,
                _ => LobbyManager.FriendFilter.Online
            };
        }

        [RelayCommand]
        private void Back()
        {
            _lobbyManager.LeaveLobby();
        }
    }
}