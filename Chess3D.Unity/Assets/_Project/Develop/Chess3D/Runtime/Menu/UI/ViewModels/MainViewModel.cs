using System;
using Chess3D.Runtime.Bootstrap;
using Chess3D.Runtime.Menu.LobbyManagement;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using LobbyManagement;
using MvvmTool;
using Noesis;
using UnityEngine;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [INotifyPropertyChanged]
    [UnityEngine.Scripting.Preserve]
    public partial class MainViewModel : ILoadUnit<IViewModel>, IDisposable
    {
        private readonly RenderTexture _renderTexture;
        private readonly LobbyManager _lobbyManager;
        private readonly AppEvents _appEvents;
        private readonly ViewNavigator _viewNavigator;

        [ObservableProperty] private IViewModel _currentViewModel;
        public ImageSource ImageSource { get; private set; }

        public MainViewModel(RenderTexture renderTexture, LobbyManager lobbyManager, AppEvents appEvents, ViewNavigator viewNavigator)
        {
            _renderTexture = renderTexture;
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

        public UniTask Load(IViewModel viewModel)
        {
            LoadStartingView(viewModel);
            InitRenderTexture();
            return UniTask.CompletedTask;
        }

        private void LoadStartingView(IViewModel viewModel)
        {
            CurrentViewModel = viewModel;
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

            ShowLobbyView();
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
            _viewNavigator.GoTo<PlayViewModel>();
        }
    }
}