using System;
using Chess3D.Runtime.Bootstrap;
using Chess3D.Runtime.Menu.LobbyManagement;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using LobbyManagement;
using Noesis;
using Ui.Auxiliary;
using Ui.Menu.Pages;
using UnityEngine;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    public class MainMenuViewModel : ILoadUnit, IDisposable
    {
        private readonly RenderTexture _renderTexture;
        private readonly LobbyManager _lobbyManager;
        private readonly AppEvents _appEvents;

        public ImageSource ImageSource { get; private set; }

        public MainMenuViewModel(RenderTexture renderTexture, LobbyManager lobbyManager, AppEvents appEvents)
        {
            _renderTexture = renderTexture;
            _lobbyManager = lobbyManager;
            _appEvents = appEvents;

            _lobbyManager.OnRoomJoined.AddListener(TryShowLobbyPage);
            _lobbyManager.OnRoomJoinFailed.AddListener(ShowOnlinePage);
            _lobbyManager.OnRoomLeft.AddListener(ShowOnlinePage);
            _appEvents.OnPause += OnApplicationPause;
        }

        public void Dispose()
        {
            if (_lobbyManager is not null)
            {
                _lobbyManager.OnRoomJoined.RemoveListener(TryShowLobbyPage);
                _lobbyManager.OnRoomJoinFailed.RemoveListener(ShowOnlinePage);
                _lobbyManager.OnRoomLeft.RemoveListener(ShowOnlinePage);
            }

            if (_appEvents is not null)
            {
                _appEvents.OnPause -= OnApplicationPause;
            }
        }

        public UniTask Load()
        {
            InitRenderTexture();
            return UniTask.CompletedTask;
        }

        private void InitRenderTexture()
        {
            if(_renderTexture)
            {
                ImageSource = new TextureSource(_renderTexture);
            }
        }

        private void OnApplicationPause(bool isPause)
        {
            if (!isPause)
            {
                return;
            }

            TryShowLobbyPage();
        }

        private void TryShowLobbyPage(Lobby _ = default)
        {
            if (IsShowLobbyPage())
            {
                ShowLobbyPage();
            }
        }

        private bool IsShowLobbyPage()
        {
            return _lobbyManager.IsInLobby
                   && !GameMenuBase.Instance.IsCurrentPage<LobbyPage>();
        }

        private static void ShowLobbyPage()
        {
            GameMenuBase.Instance.ChangePage<LobbyPage>();
        }

        private void ShowOnlinePage(string _)
        {
            ShowOnlinePage();
        }

        private void ShowOnlinePage()
        {
            GameMenuBase.Instance.ChangePage<PlayPage>("OnlineTab");
        }
    }
}