using System;
using Chess3D.Runtime.Bootstrap;
using Chess3D.Runtime.Menu.LobbyManagement;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Utilities.Logging;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer.Unity;

namespace Chess3D.Runtime.Menu.UI
{
    /// Listen to Lobby events and switch views accordingly
    [Preserve]
    public class LobbyListener : IDisposable, ITickable
    {
        // TODO: Move to config
        private const float UpdateIntervalSec = 3f;
        private const LobbyManager.FriendFilter Filter = LobbyManager.FriendFilter.Online;

        private float _lastUpdateTime;

        private readonly LobbyManager _lobbyManager;
        private readonly AppEvents _appEvents;
        private readonly ViewNavigator _viewNavigator;

        public LobbyListener(LobbyManager lobbyManager, AppEvents appEvents, ViewNavigator viewNavigator)
        {
            _lobbyManager = lobbyManager;
            _appEvents = appEvents;
            _viewNavigator = viewNavigator;

            _lobbyManager.OnRoomJoined.AddListener(ShowLobbyView);
            _lobbyManager.OnRoomJoinFailed.AddListener(OnRoomJoinFailed);
            _lobbyManager.OnRoomLeft.AddListener(GoBack);

            _appEvents.OnPause += OnApplicationPause;
        }

        public void Dispose()
        {
            if (_lobbyManager is not null)
            {
                _lobbyManager.OnRoomJoined.RemoveListener(ShowLobbyView);
                _lobbyManager.OnRoomJoinFailed.RemoveListener(OnRoomJoinFailed);
                _lobbyManager.OnRoomLeft.RemoveListener(GoBack);
            }

            if (_appEvents is not null)
            {
                _appEvents.OnPause -= OnApplicationPause;
            }
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
            _lobbyManager.PullFriends(Filter);
        }

        private void OnApplicationPause(bool isPause)
        {
            if (!isPause)
            {
                ShowLobbyView();
            }
        }

        private void ShowLobbyView(Lobby _ = default)
        {
            _viewNavigator.GoTo<LobbyViewModel>();
        }

        private static void OnRoomJoinFailed(string message)
        {
            Log.Lobby.D(message);
        }

        private void GoBack()
        {
            _viewNavigator.GoBack();
        }
    }
}