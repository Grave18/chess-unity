using System;
using Chess3D.Runtime.Menu.LobbyManagement;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Utilities.Logging;
using UnityEngine.Scripting;
using UniRx;

namespace Chess3D.Runtime.Menu.UI
{
    /// Listen to Lobby events and switch views accordingly
    [Preserve]
    public sealed class LobbyListener : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();

        public LobbyListener(LobbyManager lobbyManager, ViewNavigator viewNavigator)
        {
            lobbyManager.OnRoomJoined
                .Subscribe(_ => viewNavigator.GoTo<LobbyViewModel>())
                .AddTo(_disposables);

            lobbyManager.OnRoomLeft
                .Subscribe(_ => viewNavigator.GoBack())
                .AddTo(_disposables);

            lobbyManager.OnRoomJoinFailed
                .Subscribe(message => Log.Lobby.D(message))
                .AddTo(_disposables);

            Observable.EveryApplicationPause()
                .Subscribe(isPause =>
                {
                    if (!isPause)
                    {
                        viewNavigator.GoTo<LobbyViewModel>();
                    }
                })
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}