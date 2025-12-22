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
using Chess3D.Runtime.Core.UI.UnityGUI.LeftPanel;
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

        [Header("Sound")]
        [SerializeField] private EffectsPlayer effectsPlayerPrefab;

        [Header("UI")]
        [SerializeField] private GameCanvasEnabler gameCanvasEnabler;
        [SerializeField] private NoesisXaml mainViewXaml;
        [SerializeField] private NoesisView noesisView;

        [Header("ViewModels")]
        [SerializeField] private CoreHomeViewModel coreHomeViewModel; // Because NetworkBehaviour

        [Header("Game states")]
        [SerializeField] private GameStateMachineOnline gameStateMachineOnline; // TODO: Add online
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
            builder.Register<Game>(Lifetime.Scoped);
            builder.Register<PieceFactory>(Lifetime.Scoped);
            builder.Register<Assets>(Lifetime.Scoped);
            builder.Register<CoreEvents>(Lifetime.Scoped);
            builder.Register<Competitors>(Lifetime.Scoped);
            builder.Register<FenFromBoard>(Lifetime.Scoped);
            builder.Register<FenFromString>(Lifetime.Scoped);
            builder.Register<Highlighter>(Lifetime.Scoped);
            builder.Register<UciBuffer>(Lifetime.Scoped);
            builder.RegisterComponentInNewPrefab(effectsPlayerPrefab, Lifetime.Scoped);

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
            builder.RegisterComponent(gameCanvasEnabler);
            builder.Register<MainViewLoader>(Lifetime.Scoped);
            builder.RegisterComponent(mainViewXaml);
            builder.RegisterComponent(noesisView);
            builder.Register<ViewNavigator>(Lifetime.Scoped);
            builder.Register<MenuStateMachine>(Lifetime.Scoped);
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
            builder.RegisterEntryPoint<ClockOffline>(Lifetime.Scoped).As<IClock>(); // TODO: Add online ClockOnline
            builder.RegisterEntryPoint<GameStateMachineOffline>().As<IGameStateMachine>(); // TODO: add online GameStateMachine
            builder.RegisterEntryPoint<CoreFlow>().AsSelf();
        }
    }
}