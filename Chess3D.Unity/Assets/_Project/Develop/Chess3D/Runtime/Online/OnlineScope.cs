using Chess3D.Runtime.Core;
using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Logic.Players;
using Chess3D.Runtime.Menu.LobbyManagement;
using PurrNet;
using PurrNet.Transports;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Chess3D.Runtime.Online
{
    public class OnlineScope : LifetimeScope
    {
        [Header("Core components")]
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private PlayersDataExchanger playersDataExchanger;
        [SerializeField] private PurrTransport purrTransport;
        [SerializeField] private LocalTransport localTransport;
        [SerializeField] private UDPTransport udpTransport;

        [Header("Core components")]
        [SerializeField] private GameStateMachineOnline gameStateMachineOnline;
        [SerializeField] private ClockOnline clockOnline;
        [SerializeField] private PlayerOnline playerOnlineWhite;
        [SerializeField] private PlayerOnline playerOnlineBlack;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<CoreScope>();

            builder.RegisterInstance(FindFirstObjectByType<LobbyDataHolder>());
            builder.RegisterComponent(networkManager);
            builder.RegisterComponent(playersDataExchanger);
            builder.Register<ConnectionStarter>(Lifetime.Singleton);
            builder.Register<ConnectionTerminator>(Lifetime.Singleton).As<IInitializable>();
            builder.Register<OnlineSceneManager>(Lifetime.Singleton);
            builder.Register<OwnershipGiver>(Lifetime.Singleton).As<IInitializable>();

            builder.RegisterComponent(purrTransport);
            builder.RegisterComponent(localTransport);
            builder.RegisterComponent(udpTransport);

            builder.Register(_ => gameStateMachineOnline, Lifetime.Singleton);
            builder.Register(_ => clockOnline, Lifetime.Singleton);
            builder.Register(_ => playerOnlineWhite, Lifetime.Singleton).Keyed(PieceColor.White);
            builder.Register(_ => playerOnlineBlack, Lifetime.Singleton).Keyed(PieceColor.Black);

            builder.RegisterEntryPoint<OnlineFlow>();
        }
    }
}