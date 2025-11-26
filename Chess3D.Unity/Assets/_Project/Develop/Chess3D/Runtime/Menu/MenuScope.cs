using Chess3D.Runtime.Core;
using Chess3D.Runtime.Menu.UI.BoardInMainMenu;
using Chess3D.Runtime.Menu.UI.ViewModels;
using LobbyManagement;
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

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(renderTexture);
            builder.RegisterComponent(lobbyManager);
            builder.RegisterComponent(visualBoardSpawner);

            builder.RegisterComponentInHierarchy<NoesisView>();
            builder.Register<MainMenuViewModel>(Lifetime.Scoped);
            builder.Register<PlayPageViewModel>(Lifetime.Scoped);
            builder.Register<GameSettingsViewModel>(Lifetime.Scoped);
            builder.Register<GraphicsSettingsViewModel>(Lifetime.Scoped);
            builder.Register<AudioSettingsViewModel>(Lifetime.Scoped);
            builder.Register<FenUserControlViewModel>(Lifetime.Scoped);
            builder.Register<BoardPageViewModel>(Lifetime.Scoped);
            builder.RegisterEntryPoint<FindGameViewModel>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<LobbyViewModel>(Lifetime.Scoped).AsSelf();

            builder.RegisterBuildCallback(ServiceLocator.Initialize);

            builder.RegisterEntryPoint<MenuFlow>();
        }
    }
}