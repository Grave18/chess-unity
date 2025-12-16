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
using Chess3D.Runtime.Core.UI.UnityGUI;
using Chess3D.Runtime.Core.UI.UnityGUI.AlgebraicNotation;
using Chess3D.Runtime.Core.UI.UnityGUI.BoardUi;
using Chess3D.Runtime.Core.UI.UnityGUI.DebugUi;
using Chess3D.Runtime.Core.UI.ViewModels;
using Chess3D.Runtime.Menu.UI;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

namespace Chess3D.Runtime.Core
{
    [UnityEngine.Scripting.Preserve]
    public sealed class CoreFlow : IAsyncStartable
    {
        private readonly IObjectResolver _resolver;
        private readonly LoadingService _loadingService;
        private readonly SceneManager _sceneManager;
        private readonly FenFromString _fenFromString;
        private readonly IGameStateMachine _gameStateMachineOffline;
        private readonly MenuStateMachine _menuStateMachine;
        private readonly Assets _assets;
        private readonly Board _board;
        private readonly Game _game;
        private readonly UciBuffer _uciBuffer;
        private readonly Competitors _competitors;
        private readonly IClock _clock;
        private readonly CameraController _cameraController;
        private readonly NameCanvas _nameCanvas;
        private readonly GameSettingsViewModel _gameSettingsViewModel;
        private readonly GraphicsSettingsViewModel _graphicsSettingsViewModel;
        private readonly AudioSettingsViewModel _audioSettingsViewModel;
        private readonly FenTextBoxViewModel _fenTextBoxViewModel;
        private readonly AlgebraicNotationPanel _algebraicNotationPanel;
        private readonly UndoRedoPausePanel _undoRedoPausePanel;
        private readonly MainViewLoader _mainViewLoader;
        private readonly CoreMainViewModel _coreMainViewModel;
        private readonly CoreHomeViewModel _homeViewModel;
        private readonly ViewNavigator _viewNavigator;
        private readonly ClockPanelFactory _clockPanelFactory;

        public CoreFlow(IObjectResolver resolver, LoadingService loadingService, SceneManager sceneManager,
            FenFromString fenFromString, IGameStateMachine gameStateMachineOffline, MenuStateMachine menuStateMachine,
            Assets assets, Board board, Game game, UciBuffer uciBuffer, Competitors competitors, IClock clock,
            CameraController cameraController, NameCanvas nameCanvas,
            StateDebugPanel stateDebugPanel, BufferDebugPanel bufferDebugPanel,
            GraphicsSettingsViewModel graphicsSettingsViewModel, AudioSettingsViewModel audioSettingsViewModel,
            AlgebraicNotationPanel algebraicNotationPanel, UndoRedoPausePanel undoRedoPausePanel,
            MainViewLoader mainViewLoader, CoreMainViewModel coreMainViewModel, CoreHomeViewModel homeViewModel, ViewNavigator viewNavigator, GameSettingsViewModel gameSettingsViewModel, FenTextBoxViewModel fenTextBoxViewModel, ClockPanelFactory clockPanelFactory)
        {
            _resolver = resolver;
            _loadingService = loadingService;
            _sceneManager = sceneManager;
            _fenFromString = fenFromString;
            _gameStateMachineOffline = gameStateMachineOffline;
            _menuStateMachine = menuStateMachine;
            _assets = assets;
            _board = board;
            _game = game;
            _uciBuffer = uciBuffer;
            _competitors = competitors;
            _clock = clock;
            _cameraController = cameraController;
            _nameCanvas = nameCanvas;
            _graphicsSettingsViewModel = graphicsSettingsViewModel;
            _audioSettingsViewModel = audioSettingsViewModel;
            _algebraicNotationPanel = algebraicNotationPanel;
            _undoRedoPausePanel = undoRedoPausePanel;
            _mainViewLoader = mainViewLoader;
            _coreMainViewModel = coreMainViewModel;
            _homeViewModel = homeViewModel;
            _viewNavigator = viewNavigator;
            _gameSettingsViewModel = gameSettingsViewModel;
            _fenTextBoxViewModel = fenTextBoxViewModel;
            _clockPanelFactory = clockPanelFactory;
        }

        public async UniTask StartAsync(CancellationToken cancellation = new())
        {
            _sceneManager.SetActiveScene(RuntimeConstants.Scenes.Core);

            await _loadingService.BeginLoading(_fenFromString);
            await _loadingService.BeginLoading(_assets);
            await _loadingService.BeginLoading(_board);

            await _loadingService.BeginLoading(_uciBuffer);
            await _loadingService.BeginLoading(_game);
            await _loadingService.BeginLoading(_clock);
            await _loadingService.BeginLoading(_competitors);
            await _loadingService.BeginLoading(_cameraController);

            await _loadingService.BeginLoading(_mainViewLoader);
            await _loadingService.BeginLoading(_viewNavigator);
            await _loadingService.BeginLoading(_coreMainViewModel, _homeViewModel);
            await _loadingService.BeginLoading(_nameCanvas);
            await _loadingService.BeginLoading(_menuStateMachine);
            await _loadingService.BeginLoading(_gameSettingsViewModel);
            await _loadingService.BeginLoading(_graphicsSettingsViewModel);
            await _loadingService.BeginLoading(_audioSettingsViewModel);
            await _loadingService.BeginLoading(_fenTextBoxViewModel);
            await _loadingService.BeginLoading(_algebraicNotationPanel);
            await _loadingService.BeginLoading(_undoRedoPausePanel);

            _clockPanelFactory.Create();

            await _loadingService.BeginLoading(_gameStateMachineOffline, _resolver.Resolve<WarmUpState>());
        }
    }
}