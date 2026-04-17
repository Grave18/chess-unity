using System.Threading;
using Chess3D.Runtime.Menu.UI;
using Chess3D.Runtime.Menu.UI.UnityGUI.BoardInMainMenu;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Utilities;
using Chess3D.Runtime.Utilities.Logging;
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

        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            await _loadingService.BeginLoading(_mainViewLoader, cancellation);
            await _loadingService.BeginLoading(_viewNavigator, cancellation);
            await _loadingService.BeginLoading(_visualBoardSpawner, cancellation);
            await _loadingService.BeginLoading(_mainViewModel, _homeViewModel, cancellation);
            await _loadingService.BeginLoading(_boardViewModel, cancellation);
            await _loadingService.BeginLoading(_fenTextBoxViewModel, cancellation);
            await _loadingService.BeginLoading(_gameSettingsViewModel, cancellation);
            await _loadingService.BeginLoading(_graphicsSettingsViewModel, cancellation);
            await _loadingService.BeginLoading(_audioSettingsViewModel, cancellation);

            Log.Menu.D("MenuFlow.StartAsync()");
        }
    }
}