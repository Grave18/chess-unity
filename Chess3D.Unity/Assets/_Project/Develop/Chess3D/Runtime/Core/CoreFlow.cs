using System.Threading;
using Chess3D.Runtime.Core.AssetManagement;
using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Logic.GameStates;
using Chess3D.Runtime.Core.Logic.MenuStates;
using Chess3D.Runtime.Core.Logic.MovesBuffer;
using Chess3D.Runtime.Core.Logic.Players;
using Chess3D.Runtime.Core.MainCamera;
using Chess3D.Runtime.Core.Notation;
using Chess3D.Runtime.Core.UI.ViewModels;
using Chess3D.Runtime.Menu.UI;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Utilities;
using Chess3D.Runtime.Utilities.Logging;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Chess3D.Runtime.Core
{
    [UnityEngine.Scripting.Preserve]
    public sealed class CoreFlow : IAsyncStartable
    {
        private readonly LoadingService _loadingService;
        private readonly FenFromString _fenFromString;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly MenuStateMachine _menuStateMachine;
        private readonly UciBuffer _uciBuffer;
        private readonly Assets _assets;
        private readonly Board _board;
        private readonly Game _game;
        private readonly IClock _clock;
        private readonly Competitors _competitors;
        private readonly MainViewLoader _mainViewLoader;
        private readonly CameraController _cameraController;
        private readonly GameSettingsViewModel _gameSettingsViewModel;
        private readonly GraphicsSettingsViewModel _graphicsSettingsViewModel;
        private readonly AudioSettingsViewModel _audioSettingsViewModel;
        private readonly FenTextBoxViewModel _fenTextBoxViewModel;
        private readonly CoreMainViewModel _coreMainViewModel;
        private readonly CoreHomeViewModel _homeViewModel;
        private readonly ViewNavigator _viewNavigator;
        private readonly PlayerNames _playerNames;

        public CoreFlow(LoadingService loadingService, FenFromString fenFromString,
            IGameStateMachine gameStateMachine, MenuStateMachine menuStateMachine,
            Assets assets, Board board, Game game, UciBuffer uciBuffer, Competitors competitors,
            IClock clock, CameraController cameraController, MainViewLoader mainViewLoader,
            GraphicsSettingsViewModel graphicsSettingsViewModel, AudioSettingsViewModel audioSettingsViewModel,
            CoreMainViewModel coreMainViewModel, CoreHomeViewModel homeViewModel,
            ViewNavigator viewNavigator, GameSettingsViewModel gameSettingsViewModel,
            FenTextBoxViewModel fenTextBoxViewModel, PlayerNames playerNames)
        {
            _loadingService = loadingService;
            _fenFromString = fenFromString;
            _gameStateMachine = gameStateMachine;
            _menuStateMachine = menuStateMachine;
            _assets = assets;
            _board = board;
            _game = game;
            _uciBuffer = uciBuffer;
            _competitors = competitors;
            _clock = clock;
            _cameraController = cameraController;
            _graphicsSettingsViewModel = graphicsSettingsViewModel;
            _audioSettingsViewModel = audioSettingsViewModel;
            _mainViewLoader = mainViewLoader;
            _coreMainViewModel = coreMainViewModel;
            _homeViewModel = homeViewModel;
            _viewNavigator = viewNavigator;
            _gameSettingsViewModel = gameSettingsViewModel;
            _fenTextBoxViewModel = fenTextBoxViewModel;
            _playerNames = playerNames;
        }

        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            await _loadingService.BeginLoading(_fenFromString, cancellation);
            await _loadingService.BeginLoading(_assets, cancellation);
            await _loadingService.BeginLoading(_board, cancellation);
            await _loadingService.BeginLoading(_playerNames, cancellation);

            await _loadingService.BeginLoading(_uciBuffer, cancellation);
            await _loadingService.BeginLoading(_game, cancellation);
            await _loadingService.BeginLoading(_clock, cancellation);
            await _loadingService.BeginLoading(_competitors, cancellation);
            await _loadingService.BeginLoading(_cameraController, cancellation);

            await _loadingService.BeginLoading(_mainViewLoader, cancellation);
            await _loadingService.BeginLoading(_viewNavigator, cancellation);
            await _loadingService.BeginLoading(_coreMainViewModel, _homeViewModel, cancellation);
            await _loadingService.BeginLoading(_menuStateMachine, cancellation);
            await _loadingService.BeginLoading(_gameSettingsViewModel, cancellation);
            await _loadingService.BeginLoading(_graphicsSettingsViewModel, cancellation);
            await _loadingService.BeginLoading(_audioSettingsViewModel, cancellation);
            await _loadingService.BeginLoading(_fenTextBoxViewModel, cancellation);

            _gameStateMachine.SetState<WarmUpState>();
            _cameraController.RotateToStartPosition().Forget();

            Log.Core.D("CoreFlow.StartAsync()");
        }
    }
}