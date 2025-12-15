using System;
using Chess3D.Runtime.Bootstrap;
using Chess3D.Runtime.Menu.LobbyManagement;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Utilities.Logging;
using UnityEngine.Scripting;
using UniRx;

namespace Chess3D.Runtime.Menu.UI
{
    /// Listen to Lobby events and switch views accordingly
    [Preserve]
    public class LobbyListener : IDisposable
    {
        private readonly ViewNavigator _viewNavigator;

        private readonly CompositeDisposable _disposables = new();

        public LobbyListener(LobbyManager lobbyManager, AppEvents appEvents, ViewNavigator viewNavigator)
        {
            _viewNavigator = viewNavigator;

            lobbyManager.OnRoomJoined.Subscribe(ShowLobbyView).AddTo(_disposables);
            lobbyManager.OnRoomJoinFailed.Subscribe(OnRoomJoinFailed).AddTo(_disposables);
            lobbyManager.OnRoomLeft.Subscribe(_ => GoBack()).AddTo(_disposables);
            appEvents.OnPause.Subscribe(OnApplicationPause).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
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