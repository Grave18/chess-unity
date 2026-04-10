using System.Threading;
using Chess3D.Runtime.Menu.UI;
using Chess3D.Runtime.Menu.UI.UnityGUI.BoardInMainMenu;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Chess3D.Runtime.Menu
{
    [UnityEngine.Scripting.Preserve]
    public sealed class MenuFlow : IAsyncStartable
    {
        private readonly LoadingService _loadingService;
        private readonly VisualBoardSpawner _visualBoardSpawner;
        private readonly ViewNavigator _viewNavigator;
        private readonly MainViewLoader _mainViewLoader;
        private readonly MainViewModel _mainViewModel;
        private readonly HomeViewModel _homeViewModel;
        private readonly BoardViewModel _boardViewModel;
        private readonly FenTextBoxViewModel _fenTextBoxViewModel;
        private readonly GameSettingsViewModel _gameSettingsViewModel;
        private readonly GraphicsSettingsViewModel _graphicsSettingsViewModel;
        private readonly AudioSettingsViewModel _audioSettingsViewModel;

        public MenuFlow(LoadingService loadingService, MainViewModel mainViewModel,
            VisualBoardSpawner visualBoardSpawner, BoardViewModel boardViewModel,
            FenTextBoxViewModel fenTextBoxViewModel,
            GameSettingsViewModel gameSettingsViewModel,
            GraphicsSettingsViewModel graphicsSettingsViewModel, AudioSettingsViewModel audioSettingsViewModel,
            HomeViewModel homeViewModel, ViewNavigator viewNavigator, MainViewLoader mainViewLoader)
        {
            _loadingService = loadingService;
            _visualBoardSpawner = visualBoardSpawner;
            _boardViewModel = boardViewModel;
            _mainViewModel = mainViewModel;
            _fenTextBoxViewModel = fenTextBoxViewModel;
            _gameSettingsViewModel = gameSettingsViewModel;
            _graphicsSettingsViewModel = graphicsSettingsViewModel;
            _audioSettingsViewModel = audioSettingsViewModel;
            _homeViewModel = homeViewModel;
            _viewNavigator = viewNavigator;
            _mainViewLoader = mainViewLoader;
        }

        public async UniTask StartAsync(CancellationToken cancellation = new())
        {
            await _loadingService.BeginLoading(_mainViewLoader);
            await _loadingService.BeginLoading(_viewNavigator);
            await _loadingService.BeginLoading(_visualBoardSpawner);
            await _loadingService.BeginLoading(_mainViewModel, _homeViewModel);
            await _loadingService.BeginLoading(_boardViewModel);
            await _loadingService.BeginLoading(_fenTextBoxViewModel);
            await _loadingService.BeginLoading(_gameSettingsViewModel);
            await _loadingService.BeginLoading(_graphicsSettingsViewModel);
            await _loadingService.BeginLoading(_audioSettingsViewModel);
        }
    }
}