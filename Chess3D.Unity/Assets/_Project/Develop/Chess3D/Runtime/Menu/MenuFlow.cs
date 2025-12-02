using System.Threading;
using Chess3D.Runtime.Menu.LobbyManagement;
using Chess3D.Runtime.Menu.UI;
using Chess3D.Runtime.Menu.UI.BoardInMainMenu;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using VContainer.Unity;

namespace Chess3D.Runtime.Menu
{
    [Preserve]
    public class MenuFlow : IAsyncStartable
    {
        private readonly LoadingService _loadingService;
        private readonly LobbyManager _lobbyManager;
        private readonly VisualBoardSpawner _visualBoardSpawner;
        private readonly ViewNavigator _viewNavigator;
        private readonly MainViewLoader _mainViewLoader;
        private readonly MainViewModel _mainViewModel;
        private readonly HomeViewModel _homeViewModel;
        private readonly BoardViewModel _boardViewModel;
        private readonly PlayViewModel _playViewModel;
        private readonly FenTextBoxViewModel _fenTextBoxViewModel;
        private readonly FindGameViewModel _findGameViewModel;
        private readonly GameSettingsViewModel _gameSettingsViewModel;
        private readonly LobbyViewModel _lobbyViewModel;
        private readonly GraphicsSettingsViewModel _graphicsSettingsViewModel;
        private readonly AudioSettingsViewModel _audioSettingsViewModel;

        public MenuFlow(LoadingService loadingService, MainViewModel mainViewModel,
            VisualBoardSpawner visualBoardSpawner, BoardViewModel boardViewModel,
            FenTextBoxViewModel fenTextBoxViewModel, FindGameViewModel findGameViewModel,
            GameSettingsViewModel gameSettingsViewModel, LobbyViewModel lobbyViewModel,
            GraphicsSettingsViewModel graphicsSettingsViewModel, AudioSettingsViewModel audioSettingsViewModel,
            HomeViewModel homeViewModel, ViewNavigator viewNavigator, MainViewLoader mainViewLoader,
            LobbyListener lobbyListener, PlayViewModel playViewModel, LobbyManager lobbyManager)
        {
            _loadingService = loadingService;
            _visualBoardSpawner = visualBoardSpawner;
            _boardViewModel = boardViewModel;
            _mainViewModel = mainViewModel;
            _fenTextBoxViewModel = fenTextBoxViewModel;
            _findGameViewModel = findGameViewModel;
            _gameSettingsViewModel = gameSettingsViewModel;
            _lobbyViewModel = lobbyViewModel;
            _graphicsSettingsViewModel = graphicsSettingsViewModel;
            _audioSettingsViewModel = audioSettingsViewModel;
            _homeViewModel = homeViewModel;
            _viewNavigator = viewNavigator;
            _mainViewLoader = mainViewLoader;
            _playViewModel = playViewModel;
            _lobbyManager = lobbyManager;
        }

        public async UniTask StartAsync(CancellationToken cancellation = new())
        {
            await _loadingService.BeginLoading(_lobbyManager);
            await _loadingService.BeginLoading(_mainViewLoader);
            await _loadingService.BeginLoading(_viewNavigator);
            await _loadingService.BeginLoading(_visualBoardSpawner);
            await _loadingService.BeginLoading(_mainViewModel, _homeViewModel);
            await _loadingService.BeginLoading(_boardViewModel);
            await _loadingService.BeginLoading(_fenTextBoxViewModel);
            await _loadingService.BeginLoading(_findGameViewModel);
            await _loadingService.BeginLoading(_gameSettingsViewModel);
            await _loadingService.BeginLoading(_playViewModel);
            await _loadingService.BeginLoading(_lobbyViewModel);
            await _loadingService.BeginLoading(_graphicsSettingsViewModel);
            await _loadingService.BeginLoading(_audioSettingsViewModel);
        }
    }
}