using Chess3D.Runtime.Core;
using Chess3D.Runtime.Menu.UI;
using Chess3D.Runtime.Menu.UI.BoardInMainMenu;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Menu.UI.Views;
using LobbyManagement;
using Ui.Menu.Pages;
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
        [SerializeField] private MainViewLoader mainViewLoader;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(renderTexture);
            builder.RegisterComponent(lobbyManager);
            builder.RegisterComponent(visualBoardSpawner);

            // UI
            builder.RegisterComponent(mainViewLoader); // Load MainView
            builder.Register<ViewNavigator>(Lifetime.Scoped); // Navigate through Views
            // Views
            builder.Register<HomeView>(Lifetime.Scoped);
            builder.Register<BoardView>(Lifetime.Scoped);
            builder.Register<PlayView>(Lifetime.Scoped);
            builder.Register<SettingsView>(Lifetime.Scoped);
            builder.Register<FindGamePage>(Lifetime.Scoped);
            builder.Register<LobbyPage>(Lifetime.Scoped);
            // VMs
            builder.Register<MainViewModel>(Lifetime.Scoped);
            builder.Register<HomeViewModel>(Lifetime.Scoped);
            builder.Register<BoardViewModel>(Lifetime.Scoped);
            builder.Register<PlayViewModel>(Lifetime.Scoped);
            builder.Register<SettingsViewModel>(Lifetime.Scoped);
            builder.Register<GameSettingsViewModel>(Lifetime.Scoped);
            builder.Register<GraphicsSettingsViewModel>(Lifetime.Scoped);
            builder.Register<AudioSettingsViewModel>(Lifetime.Scoped);
            builder.Register<FenTextBoxViewModel>(Lifetime.Scoped);
            builder.RegisterEntryPoint<FindGameViewModel>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<LobbyViewModel>(Lifetime.Scoped).AsSelf();

            builder.RegisterBuildCallback(ServiceLocator.Initialize);

            builder.RegisterEntryPoint<MenuFlow>();
        }
    }
}