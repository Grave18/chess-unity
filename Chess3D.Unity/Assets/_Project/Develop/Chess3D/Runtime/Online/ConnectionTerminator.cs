using Chess3D.Runtime.Bootstrap.Settings;
using Cysharp.Threading.Tasks;
using PurrNet;
using PurrNet.Transports;
using UnityEngine;

namespace Chess3D.Runtime.Online
{
    public class ConnectionTerminator : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private OnlineSceneLoader onlineSceneLoader;

        public static void DisconnectFromServer()
        {
            InstanceHandler.NetworkManager?.StopClient();
        }

        private void Awake()
        {
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.onClientConnectionState += OnClientConnectionState;
            }
        }

        protected override void OnDestroy()
        {
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.onClientConnectionState -= OnClientConnectionState;
            }
        }

        private void OnClientConnectionState(ConnectionState state)
        {
            if (state == ConnectionState.Disconnected)
            {
                if (isServer)
                {
                    HostDisconnect().Forget();
                }
                else
                {
                    ClientDisconnect();
                }
            }
        }

        private async UniTaskVoid HostDisconnect()
        {
            await onlineSceneLoader.UnloadGameAndLoadMainMenu();

            InstanceHandler.NetworkManager?.StopServer();
        }

        // Add delay to game scene to unload and load main menu
        private void ClientDisconnect()
        {
            onlineSceneLoader.UnloadGameAndLoadMainMenu().Forget();
        }
    }
}