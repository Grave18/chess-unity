using UnityEngine;
using VContainer;
using VContainer.Unity;
using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.AI;
using Chess3D.Runtime.Core.AssetManagement;
using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.Logic;
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

namespace Chess3D.Runtime.Core
{
    public sealed class CoreScope : LifetimeScope
    {
        [Header("Game")]
        [SerializeField] private Board board;
        [SerializeField] private GameStateMachineOffline gameStateMachineOfflinePrefab;

        [Header("Sound")]
        [SerializeField] private EffectsPlayer effectsPlayerPrefab;

        [Header("UI")]
        [SerializeField] private GameCanvasEnabler gameCanvasEnabler;
        [SerializeField] private NoesisXaml mainViewXaml;
        [SerializeField] private NoesisView noesisView;

        [Header("ViewModels")]
        [SerializeField] private CoreHomeViewModel coreHomeViewModel; // Because NetworkBehaviour

        [Header("Camera")]
        [SerializeField] private CameraController cameraController;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Camera uiCamera;

        [Header("Online")]
        [SerializeField] private OnlineScope onlineScopePrefab;
        [SerializeField] private GameStateMachineOnline gameStateMachineOnlinePrefab;
        [SerializeField] private ClockOnline clockOnlinePrefab;
        [SerializeField] private PlayerOnline playerOnlinePrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(board);
            builder.Register<Game>(Lifetime.Singleton);
            builder.Register<PieceFactory>(Lifetime.Singleton);
            builder.Register<Assets>(Lifetime.Singleton);
            builder.Register<CoreEvents>(Lifetime.Singleton);
            builder.Register<Competitors>(Lifetime.Singleton);
            builder.Register<FenFromBoard>(Lifetime.Singleton);
            builder.Register<FenFromString>(Lifetime.Singleton);
            builder.Register<UciBuffer>(Lifetime.Singleton);
            builder.RegisterComponentInNewPrefab(effectsPlayerPrefab, Lifetime.Singleton);

            // Player
            builder.RegisterComponentInNewPrefab(gameStateMachineOfflinePrefab, Lifetime.Singleton);
            builder.Register<PlayerFactory>(Lifetime.Singleton);
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
            builder.Register<StartEndSoundPlayer>(Lifetime.Singleton);

            // UI
            builder.RegisterComponent(gameCanvasEnabler);
            builder.Register<MainViewLoader>(Lifetime.Singleton);
            builder.RegisterComponent(mainViewXaml);
            builder.RegisterComponent(noesisView);
            builder.Register<ViewNavigator>(Lifetime.Singleton);
            builder.Register<MenuStateMachine>(Lifetime.Singleton);
            // Views
            builder.Register<CoreHomeView>(Lifetime.Singleton);
            builder.Register<SettingsView>(Lifetime.Singleton);
            // ViewModels
            builder.Register<CoreMainViewModel>(Lifetime.Singleton).As<IMainViewModel>().AsSelf();
            builder.RegisterComponent(coreHomeViewModel); // Network behaviour
            builder.Register<PopupViewModel>(Lifetime.Singleton);
            // Settings VMs
            builder.Register<SettingsViewModel>(Lifetime.Singleton);
            builder.Register<GameSettingsViewModel>(Lifetime.Singleton);
            builder.Register<GraphicsSettingsViewModel>(Lifetime.Singleton);
            builder.Register<AudioSettingsViewModel>(Lifetime.Singleton);
            builder.Register<FenTextBoxViewModel>(Lifetime.Singleton);

            // Online
            builder.RegisterComponentInNewPrefab(onlineScopePrefab, Lifetime.Singleton); // Instantiated in Flow
            builder.RegisterComponentInNewPrefab(clockOnlinePrefab, Lifetime.Singleton); // Conditional registration
            builder.RegisterComponentInNewPrefab(gameStateMachineOnlinePrefab, Lifetime.Singleton); // Conditional registration
            builder.RegisterComponentInNewPrefab(playerOnlinePrefab, Lifetime.Transient); // Instantiated in PlayerFactory

            builder.Register<IClock>(resolver =>
            {
                var configContainer = resolver.Resolve<ConfigContainer>();
                return configContainer.OnlineConfig.ConnectionType == ConnectionType.Online
                    ? resolver.Resolve<ClockOnline>()
                    : resolver.Resolve<ClockOffline>();
            }, Lifetime.Singleton);

            builder.Register(resolver =>
            {
                var configContainer = resolver.Resolve<ConfigContainer>();
                IGameStateMachine gameStateMachine = configContainer.OnlineConfig.ConnectionType == ConnectionType.Online
                    ? resolver.Resolve<GameStateMachineOnline>()
                    : resolver.Resolve<GameStateMachineOffline>();

                return gameStateMachine;
            }, Lifetime.Singleton);

            // Service Locator
            builder.RegisterBuildCallback(ServiceLocator.Initialize);

            // Entry points
            builder.RegisterEntryPoint<ClockOffline>().As<IClock>(); // TODO: Add online ClockOnline
            builder.RegisterEntryPoint<CoreFlow>().AsSelf();
        }
    }
}