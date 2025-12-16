using UnityEngine;
using VContainer;
using VContainer.Unity;
using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.AI;
using Chess3D.Runtime.Core.AssetManagement;
using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.Highlighting;
using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Logic.GameStates;
using Chess3D.Runtime.Core.Logic.MenuStates;
using Chess3D.Runtime.Core.Logic.MovesBuffer;
using Chess3D.Runtime.Core.Logic.Players;
using Chess3D.Runtime.Core.MainCamera;
using Chess3D.Runtime.Core.Notation;
using Chess3D.Runtime.Core.Sound;
using Chess3D.Runtime.Core.UI.UnityGUI;
using Chess3D.Runtime.Core.UI.UnityGUI.AlgebraicNotation;
using Chess3D.Runtime.Core.UI.UnityGUI.BoardUi;
using Chess3D.Runtime.Core.UI.UnityGUI.ClockUi;
using Chess3D.Runtime.Core.UI.UnityGUI.DebugUi;
using Chess3D.Runtime.Core.UI.UnityGUI.Promotion;
using Chess3D.Runtime.Core.UI.ViewModels;
using Chess3D.Runtime.Core.UI.Views;
using Chess3D.Runtime.Menu.UI;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Menu.UI.Views;
using Chess3D.Runtime.Online;
using Chess3D.Runtime.Utilities;
using PauseState = Chess3D.Runtime.Core.Logic.GameStates.PauseState;

namespace Chess3D.Runtime.Core
{
    public sealed class CoreScope : LifetimeScope
    {
        [Header("Game")]
        [SerializeField] private Board board;
        [SerializeField] private ClockOffline clockOffline; // TODO: Make entry point, Factory. used now
        [SerializeField] private GameStateMachineOnline gameStateMachineOnline; // TODO: Factory

        [Header("Sound")]
        [SerializeField] private EffectsPlayer effectsPlayer;

        [Header("Ui")]
        [SerializeField] public NoesisXaml mainViewXaml;
        [SerializeField] private NoesisView noesisView;
        [SerializeField] private GameCanvas gameCanvas;
        [SerializeField] private NameCanvas nameCanvas;
        [SerializeField] private StateDebugPanel stateDebugPanelPrefab;
        [SerializeField] private BufferDebugPanel bufferDebugPanelPrefab;
        [SerializeField] private ClockPanelCanvas whiteClockPrefab;
        [SerializeField] private ClockPanelCanvas blackClockPrefab;
        [SerializeField] private PromotionPanel promotionPanel;
        [SerializeField] private NotificationPanel notificationPanel;
        [SerializeField] private AlgebraicNotationPanel algebraicNotationPanel;
        [SerializeField] private UndoRedoPausePanel undoRedoPausePanel;

        [Header("ViewModels")]
        [SerializeField] private CoreHomeViewModel coreHomeViewModel; // Because NetworkBehaviour

        [Header("Game states")]
        [SerializeField] private WarmUpState warmupState;
        [SerializeField] private IdleState idleState;
        [SerializeField] private MoveState moveState;
        [SerializeField] private PauseState pauseState;
        [SerializeField] private UndoState undoState;
        [SerializeField] private RedoState redoState;
        [SerializeField] private EndGameState endGameState;

        [Header("Camera")]
        [SerializeField] private CameraController cameraController;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Camera uiCamera;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(board);
            builder.RegisterComponent(clockOffline).As<IClock>();
            builder.Register<Game>(Lifetime.Scoped);
            builder.Register<PieceFactory>(Lifetime.Scoped);
            builder.Register<Assets>(Lifetime.Scoped);
            builder.Register<CoreEvents>(Lifetime.Scoped);
            builder.Register<Competitors>(Lifetime.Scoped);
            builder.Register<FenFromBoard>(Lifetime.Scoped);
            builder.Register<FenFromString>(Lifetime.Scoped);
            builder.Register<Highlighter>(Lifetime.Scoped);
            builder.Register<UciBuffer>(Lifetime.Scoped);
            builder.RegisterComponent(effectsPlayer);

            // State Machines
            builder.RegisterComponent(gameStateMachineOnline).AsSelf();
            builder.RegisterComponent(warmupState).AsSelf();
            builder.RegisterComponent(idleState).AsSelf();
            builder.RegisterComponent(moveState).AsSelf();
            builder.RegisterComponent(pauseState).AsSelf();
            builder.RegisterComponent(undoState).AsSelf();
            builder.RegisterComponent(redoState).AsSelf();
            builder.RegisterComponent(endGameState).AsSelf();
            builder.Register<CommonPieceSettings>(Lifetime.Scoped);

            // Player
            builder.Register<PlayerFactory>(Lifetime.Scoped);
            builder.Register<InputHuman>(Lifetime.Transient).As<IInputHandler>().Keyed(PlayerType.Human);
            builder.Register<InputComputer>(Lifetime.Transient).As<IInputHandler>().Keyed(PlayerType.Computer);
            // AI
            builder.Register<Stockfish>(Lifetime.Transient);

            // Camera
            builder.RegisterComponent(cameraController);
            builder.RegisterComponent(mainCamera).AsSelf();
            builder.RegisterComponent(mainCamera).AsSelf().Keyed(CameraKeys.Main);
            builder.RegisterComponent(uiCamera).AsSelf().Keyed(CameraKeys.Ui);

            // Sound
            builder.Register<StartEndSoundPlayer>(Lifetime.Scoped);

            // UI
            builder.Register<MainViewLoader>(Lifetime.Scoped);
            builder.RegisterComponent(mainViewXaml);
            builder.RegisterComponent(noesisView);
            builder.Register<ViewNavigator>(Lifetime.Scoped);
            builder.Register<MenuStateMachine>(Lifetime.Scoped);
            builder.RegisterComponent(gameCanvas);
            builder.RegisterComponent(nameCanvas);
            builder.RegisterComponent(promotionPanel);
            builder.RegisterComponent(notificationPanel);
            builder.RegisterComponent(algebraicNotationPanel);
            builder.RegisterComponent(undoRedoPausePanel);
            // Clock UI
            builder.Register<ClockPanelFactory>(Lifetime.Scoped);
            builder.RegisterComponentInNewPrefab(whiteClockPrefab, Lifetime.Scoped).Keyed(PieceColor.White);
            builder.RegisterComponentInNewPrefab(blackClockPrefab, Lifetime.Scoped).Keyed(PieceColor.Black);
            // Views
            builder.Register<CoreHomeView>(Lifetime.Scoped);
            builder.Register<SettingsView>(Lifetime.Scoped);
            // ViewModels
            builder.Register<CoreMainViewModel>(Lifetime.Scoped).As<IMainViewModel>().AsSelf();
            builder.RegisterComponent(coreHomeViewModel); // Network behaviour
            builder.Register<PopupViewModel>(Lifetime.Scoped);
            // Settings VMs
            builder.Register<SettingsViewModel>(Lifetime.Scoped);
            builder.Register<GameSettingsViewModel>(Lifetime.Scoped);
            builder.Register<GraphicsSettingsViewModel>(Lifetime.Scoped);
            builder.Register<AudioSettingsViewModel>(Lifetime.Scoped);
            builder.Register<FenTextBoxViewModel>(Lifetime.Scoped);

            // Online
            builder.RegisterComponentOnNewGameObject<ConnectionTerminator>(Lifetime.Scoped);

            // Service Locator
            builder.RegisterBuildCallback(ServiceLocator.Initialize);

            // Entry points
            builder.RegisterEntryPoint<GameStateMachineOffline>().As<IGameStateMachine>();
            builder.RegisterEntryPoint<CoreFlow>().AsSelf();

#if !GRAVEBOX_PROD
            RegisterDebugUI(builder);
#endif
        }

        private void RegisterDebugUI(IContainerBuilder builder)
        {
            builder.RegisterComponentInNewPrefab(stateDebugPanelPrefab, Lifetime.Scoped).UnderTransform(gameCanvas.transform);
            builder.RegisterComponentInNewPrefab(bufferDebugPanelPrefab, Lifetime.Scoped).UnderTransform(gameCanvas.transform);
        }
    }
}