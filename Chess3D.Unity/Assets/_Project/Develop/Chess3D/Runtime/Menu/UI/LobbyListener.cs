using System;
using Chess3D.Runtime.Bootstrap;
using Chess3D.Runtime.Menu.LobbyManagement;
using Chess3D.Runtime.Menu.UI.ViewModels;
using LobbyManagement;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI
{
    /// Listen to Lobby events and switch views accordingly
    [Preserve]
    public class LobbyListener : IDisposable
    {
        private readonly LobbyManager _lobbyManager;
        private readonly AppEvents _appEvents;
        private readonly ViewNavigator _viewNavigator;

        public LobbyListener(LobbyManager lobbyManager, AppEvents appEvents, ViewNavigator viewNavigator)
        {
            _lobbyManager = lobbyManager;
            _appEvents = appEvents;
            _viewNavigator = viewNavigator;

            _lobbyManager.OnRoomJoined.AddListener(ShowLobbyView);
            _lobbyManager.OnRoomJoinFailed.AddListener(ShowPlayView);
            _lobbyManager.OnRoomLeft.AddListener(ShowPlayView);
            _appEvents.OnPause += OnApplicationPause;
        }

        public void Dispose()
        {
            if (_lobbyManager is not null)
            {
                _lobbyManager.OnRoomJoined.RemoveListener(ShowLobbyView);
                _lobbyManager.OnRoomJoinFailed.RemoveListener(ShowPlayView);
                _lobbyManager.OnRoomLeft.RemoveListener(ShowPlayView);
            }

            if (_appEvents is not null)
            {
                _appEvents.OnPause -= OnApplicationPause;
            }
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

        private void ShowPlayView(string _)
        {
            ShowPlayView();
        }

        private void ShowPlayView()
        {
            _viewNavigator.GoBack();
        }
    }
}