using System;
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
using Chess3D.Runtime.Core.Ui.BoardUi;
using Chess3D.Runtime.Core.Ui.ClockUi;
using Chess3D.Runtime.Core.Ui.DebugUi;
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
        private readonly IInputHandler _inputHandler;
        private readonly GraphicsSettingsViewModel _graphicsSettingsViewModel;

        public CoreFlow(IObjectResolver resolver, LoadingService loadingService, SceneManager sceneManager,
            FenFromString fenFromString, IGameStateMachine gameStateMachineOffline, MenuStateMachine menuStateMachine,
            Assets assets, Board board, Game game, UciBuffer uciBuffer, Competitors competitors, IClock clock,
            CameraController cameraController, NameCanvas nameCanvas, IInputHandler inputHandler,
            StateDebugPanel stateDebugPanel, BufferDebugPanel bufferDebugPanel, GraphicsSettingsViewModel graphicsSettingsViewModel)
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
            _inputHandler = inputHandler;
            _graphicsSettingsViewModel = graphicsSettingsViewModel;
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

            await _loadingService.BeginLoading(_inputHandler);

            await _loadingService.BeginLoading(_nameCanvas);
            await _loadingService.BeginLoading(_menuStateMachine);
            await _loadingService.BeginLoading(_graphicsSettingsViewModel);
            SpawnClockPanel();

            await _loadingService.BeginLoading(_gameStateMachineOffline, _resolver.Resolve<WarmUpState>());
        }

        private void SpawnClockPanel()
        {
            var clockPanelFactory = _resolver.Resolve<Func<ClockPanelCanvas>>();
            clockPanelFactory();
        }
    }
}