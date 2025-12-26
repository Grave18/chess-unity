using Chess3D.Runtime.Menu.LobbyManagement;
using PurrNet;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Chess3D.Runtime.Online
{
    public class OnlineScope : LifetimeScope
    {
        [Header("References")]
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private ConnectionStarter connectionStarter;
        [SerializeField] private PlayersDataExchanger playersDataExchanger;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(FindFirstObjectByType<LobbyDataHolder>());
            builder.RegisterComponent(networkManager);
            builder.RegisterComponent(connectionStarter);
            builder.RegisterComponent(playersDataExchanger);

            builder.RegisterEntryPoint<OnlineFlow>(Lifetime.Scoped);
        }
    }
}