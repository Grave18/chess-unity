using UnityEngine;
using VContainer;
using VContainer.Unity;
using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.AI;
using Chess3D.Runtime.Core.AssetManagement;
using Chess3D.Runtime.Core.ChessBoard;
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
using Chess3D.Runtime.Utilities;
using PauseState = Chess3D.Runtime.Core.Logic.GameStates.PauseState;

namespace Chess3D.Runtime.Core
{
    public sealed class CoreScope : LifetimeScope
    {
        [Header("Game")]
        [SerializeField] private Board board;

        [Header("Sound")]
        [SerializeField] private EffectsPlayer effectsPlayer;

        [Header("UI")]
        [SerializeField] private GameCanvasEnabler gameCanvasEnabler;
        [SerializeField] private NoesisXaml mainViewXaml;
        [SerializeField] private NoesisView noesisView;

        [Header("Camera")]
        [SerializeField] private CameraController cameraController;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Camera uiCamera;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(board);
            builder.RegisterComponent(effectsPlayer);
            builder.Register<Game>(Lifetime.Singleton);
            builder.Register<PieceFactory>(Lifetime.Singleton);
            builder.Register<Assets>(Lifetime.Singleton);
            builder.Register<CoreEvents>(Lifetime.Singleton);
            builder.Register<Competitors>(Lifetime.Singleton);
            builder.Register<FenFromBoard>(Lifetime.Singleton);
            builder.Register<FenFromString>(Lifetime.Singleton);
            builder.Register<UciBuffer>(Lifetime.Singleton);

            // Player
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
            builder.Register<PlayerNames>(Lifetime.Singleton);
            // Views
            builder.Register<CoreHomeView>(Lifetime.Singleton);
            builder.Register<SettingsView>(Lifetime.Singleton);
            // ViewModels
            builder.Register<CoreMainViewModel>(Lifetime.Singleton).As<IMainViewModel>().AsSelf();
            builder.Register<CoreHomeViewModel>(Lifetime.Singleton); // Network behaviour
            builder.Register<PopupViewModel>(Lifetime.Singleton);
            // Settings VMs
            builder.Register<SettingsViewModel>(Lifetime.Singleton);
            builder.Register<GameSettingsViewModel>(Lifetime.Singleton);
            builder.Register<GraphicsSettingsViewModel>(Lifetime.Singleton);
            builder.Register<AudioSettingsViewModel>(Lifetime.Singleton);
            builder.Register<FenTextBoxViewModel>(Lifetime.Singleton);

            builder.Register(RegisterClock, Lifetime.Singleton);
            builder.Register<InitialState>(Lifetime.Singleton);
            builder.Register<WarmUpState>(Lifetime.Singleton);
            builder.Register<MoveState>(Lifetime.Singleton);
            builder.Register<IdleState>(Lifetime.Singleton);
            builder.Register<EndGameState>(Lifetime.Singleton);
            builder.Register<PlayState>(Lifetime.Singleton);
            builder.Register<PauseState>(Lifetime.Singleton);
            builder.Register<UndoState>(Lifetime.Singleton);
            builder.Register<RedoState>(Lifetime.Singleton);

            // Service Locator
            builder.RegisterBuildCallback(ServiceLocator.Initialize);

            builder.Register<Clock>(Lifetime.Singleton);
            builder.Register<GameStateMachine>(Lifetime.Singleton).As<IGameStateMachine>();

            // Entry points
            builder.RegisterEntryPoint<CoreFlow>().AsSelf();
        }

        private static IClock RegisterClock(IObjectResolver resolver)
        {
            resolver.Resolve<ConfigContainer>();
            IClock clock = resolver.Resolve<Clock>();

            return clock;
        }
    }
}