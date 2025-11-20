using System;
using Chess3D.Runtime.Core.Ai;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Chess3D.Runtime.Core.Ui.ViewModels;
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
using Chess3D.Runtime.Core.Ui;
using Chess3D.Runtime.Core.Ui.BoardUi;
using Chess3D.Runtime.Core.Ui.ClockUi;
using Chess3D.Runtime.Core.Ui.Promotion;
using Chess3D.Runtime.Online;
using PauseState = Chess3D.Runtime.Core.Logic.GameStates.PauseState;

namespace Chess3D.Runtime.Core
{
    public sealed class CoreScope : LifetimeScope
    {
        [Header("Game")]
        [SerializeField] private Board board;
        [SerializeField] private ClockOffline clockOffline; // TODO: Make entry point, Fabric. used now
        [SerializeField] private GameStateMachineOnline gameStateMachineOnline; // TODO: Fabric

        [SerializeField] private EffectsPlayer effectsPlayer;

        [Header("Ui")]
        [SerializeField] private GameCanvas gameCanvas;
        [SerializeField] private NameCanvas nameCanvas;
        [SerializeField] private PromotionPanel promotionPanel;
        [SerializeField] private NotificationPanel notificationPanel;
        [SerializeField] private ClockPanelCanvas whiteClockPrefab;
        [SerializeField] private ClockPanelCanvas blackClockPrefab;
        [SerializeField] private InGamePageViewModel inGamePageViewModel; // Because NetworkBehaviour

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
            builder.RegisterEntryPoint<GameStateMachineOffline>().As<IGameStateMachine>();
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
            builder.Register<InputHuman>(Lifetime.Transient).As<IInputHandler>();
            builder.RegisterFactory(PlayerFactory, Lifetime.Transient);
            // AI
            builder.Register<Stockfish>(Lifetime.Transient);

            // Camera
            builder.RegisterComponent(cameraController);
            builder.RegisterComponent(mainCamera).AsSelf();
            builder.RegisterComponent(mainCamera).AsSelf().Keyed(CameraKeys.Main);
            builder.RegisterComponent(uiCamera).AsSelf().Keyed(CameraKeys.Ui);

            // Sound
            builder.Register<StartEndSoundPlayer>(Lifetime.Scoped);

            // Ui
            builder.Register<MenuStateMachine>(Lifetime.Scoped);
            builder.RegisterComponent(gameCanvas);
            builder.RegisterComponent(nameCanvas);
            builder.RegisterComponent(promotionPanel);
            builder.RegisterComponent(notificationPanel);
            // Clock Ui
            builder.RegisterFactory(ClockPanelFactory, Lifetime.Scoped);
            builder.RegisterComponentInNewPrefab(whiteClockPrefab, Lifetime.Scoped).Keyed(PieceColor.White);
            builder.RegisterComponentInNewPrefab(blackClockPrefab, Lifetime.Scoped).Keyed(PieceColor.Black);
            // VMs
            builder.RegisterComponent(inGamePageViewModel);
            builder.Register<InGameMenuViewModel>(Lifetime.Scoped);
            builder.Register<PopupViewModel>(Lifetime.Scoped);

            // Online
            builder.RegisterComponentOnNewGameObject<ConnectionTerminator>(Lifetime.Scoped);

            builder.RegisterEntryPoint<CoreFlow>();
        }

        private static Func<IPlayer> PlayerFactory(IObjectResolver resolver)
        {
            return () =>
            {
                var input = resolver.Resolve<IInputHandler>();
                return new PlayerOffline(input);
            };
        }

        private static Func<ClockPanelCanvas> ClockPanelFactory(IObjectResolver resolver)
        {
            return () =>
            {
                var settingsService = resolver.Resolve<SettingsService>();
                var game = resolver.Resolve<Game>();
                var clock = resolver.Resolve<IClock>();
                var uiCamera = resolver.Resolve<Camera>(CameraKeys.Ui);
                var coreEvents = resolver.Resolve<CoreEvents>();
                var instance = resolver.Resolve<ClockPanelCanvas>(settingsService.S.GameSettings.PlayerColor);
                instance.Construct(game, coreEvents, clock, uiCamera);

                return instance;
            };
        }
    }
}