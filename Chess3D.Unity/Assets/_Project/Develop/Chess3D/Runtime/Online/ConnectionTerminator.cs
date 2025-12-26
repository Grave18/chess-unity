using Cysharp.Threading.Tasks;
using PurrNet;
using PurrNet.Transports;
using UnityEngine;

namespace Chess3D.Runtime.Online
{
    public class ConnectionTerminator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private OnlineSceneManager onlineSceneManager;

        private void Awake()
        {
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.onClientConnectionState += OnClientConnectionState;
            }
        }

        private void OnDestroy()
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
                if (InstanceHandler.NetworkManager?.isServer == true)
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
            await onlineSceneManager.LoadMenu();

            InstanceHandler.NetworkManager?.StopServer();
        }

        // Add delay to game scene to unload and load main menu
        private void ClientDisconnect()
        {
            onlineSceneManager.LoadMenu().Forget();
        }
    }
}