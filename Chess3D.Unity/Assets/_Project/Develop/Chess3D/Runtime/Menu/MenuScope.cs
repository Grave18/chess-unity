using Chess3D.Runtime.Core;
using Chess3D.Runtime.Menu.LobbyManagement;
using Chess3D.Runtime.Menu.UI;
using Chess3D.Runtime.Menu.UI.BoardInMainMenu;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Menu.UI.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Chess3D.Runtime.Menu
{
    public class MenuScope : LifetimeScope
    {
        [SerializeField] private RenderTexture renderTexture;
        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private VisualBoardSpawner visualBoardSpawner;
        [SerializeField] public NoesisXaml mainViewXaml;
        [SerializeField] private NoesisView noesisView;

        protected override void Configure(IContainerBuilder builder)
        {
            // Lobby
            builder.RegisterComponent(lobbyManager);
            builder.Register<LobbyListener>(Lifetime.Scoped);

            // UI
            builder.RegisterComponent(renderTexture);
            builder.RegisterComponent(visualBoardSpawner);
            builder.RegisterComponent(mainViewXaml);
            builder.RegisterComponent(noesisView);
            builder.Register<MainViewLoader>(Lifetime.Scoped);
            builder.Register<ViewNavigator>(Lifetime.Scoped);
            // Views
            builder.Register<HomeView>(Lifetime.Scoped);
            builder.Register<BoardView>(Lifetime.Scoped);
            builder.Register<PlayView>(Lifetime.Scoped);
            builder.Register<SettingsView>(Lifetime.Scoped);
            builder.Register<LobbyView>(Lifetime.Scoped);
            // ViewModels
            builder.Register<MainViewModel>(Lifetime.Scoped);
            builder.Register<HomeViewModel>(Lifetime.Scoped);
            builder.Register<BoardViewModel>(Lifetime.Scoped);
            builder.Register<PlayViewModel>(Lifetime.Scoped);
            builder.Register<SettingsViewModel>(Lifetime.Scoped);
            builder.Register<GameSettingsViewModel>(Lifetime.Scoped);
            builder.Register<GraphicsSettingsViewModel>(Lifetime.Scoped);
            builder.Register<AudioSettingsViewModel>(Lifetime.Scoped);
            builder.Register<FenTextBoxViewModel>(Lifetime.Scoped);
            builder.Register<LobbyViewModel>(Lifetime.Scoped);

            // Service locator
            builder.RegisterBuildCallback(ServiceLocator.Initialize);

            // Entry points
            builder.RegisterEntryPoint<LobbyViewModel>(Lifetime.Scoped);
            builder.RegisterEntryPoint<MenuFlow>();
        }
    }
}