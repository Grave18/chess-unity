using System.Threading;
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
        private readonly VisualBoardSpawner _visualBoardSpawner;
        private readonly MainMenuViewModel _mainMenuViewModel;
        private readonly BoardPageViewModel _boardPageViewModel;
        private readonly FenUserControlViewModel _fenUserControlViewModel;
        private readonly FindGameViewModel _findGameViewModel;
        private readonly GameSettingsViewModel _gameSettingsViewModel;
        private readonly LobbyViewModel _lobbyViewModel;
        private readonly GraphicsSettingsViewModel _graphicsSettingsViewModel;
        private readonly AudioSettingsViewModel _audioSettingsViewModel;

        public MenuFlow(LoadingService loadingService, MainMenuViewModel mainMenuViewModel,
            VisualBoardSpawner visualBoardSpawner, BoardPageViewModel boardPageViewModel,
            FenUserControlViewModel fenUserControlViewModel, FindGameViewModel findGameViewModel,
            GameSettingsViewModel gameSettingsViewModel, LobbyViewModel lobbyViewModel, GraphicsSettingsViewModel graphicsSettingsViewModel, AudioSettingsViewModel audioSettingsViewModel)
        {
            _loadingService = loadingService;
            _visualBoardSpawner = visualBoardSpawner;
            _boardPageViewModel = boardPageViewModel;
            _mainMenuViewModel = mainMenuViewModel;
            _fenUserControlViewModel = fenUserControlViewModel;
            _findGameViewModel = findGameViewModel;
            _gameSettingsViewModel = gameSettingsViewModel;
            _lobbyViewModel = lobbyViewModel;
            _graphicsSettingsViewModel = graphicsSettingsViewModel;
            _audioSettingsViewModel = audioSettingsViewModel;
        }

        public async UniTask StartAsync(CancellationToken cancellation = new())
        {
            await _loadingService.BeginLoading(_visualBoardSpawner);
            await _loadingService.BeginLoading(_mainMenuViewModel);
            await _loadingService.BeginLoading(_boardPageViewModel);
            await _loadingService.BeginLoading(_fenUserControlViewModel);
            await _loadingService.BeginLoading(_findGameViewModel);
            await _loadingService.BeginLoading(_gameSettingsViewModel);
            await _loadingService.BeginLoading(_lobbyViewModel);
            await _loadingService.BeginLoading(_graphicsSettingsViewModel);
            await _loadingService.BeginLoading(_audioSettingsViewModel);
        }
    }
}