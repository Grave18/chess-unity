using System.Threading.Tasks;
using LobbyManagement;
using PurrNet;
using PurrNet.Logging;
using PurrNet.Transports;
using Settings;
using UnityEngine;

namespace Network
{
    public class ConnectionStarter : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;

        private void CheckReferences(LobbyDataHolder lobbyDataHolder)
        {
            if (!networkManager)
            {
                PurrLogger.LogError($"Failed to start connection. {nameof(NetworkManager)} is null!", this);
            }

            if (!lobbyDataHolder)
            {
                PurrLogger.LogError($"Failed to start connection. {nameof(LobbyDataHolder)} is null!", this);
            }

            if (!lobbyDataHolder.CurrentLobby.IsValid)
            {
                PurrLogger.Log("Lobby is not valid.", this);
            }
        }

        public void SetupTransports(Lobby currentLobby)
        {
            if(networkManager.transport is CompositeTransport composite)
            {
                foreach (GenericTransport transport in composite.transports)
                {
                    if(transport is PurrTransport purrTransport)
                    {
                        SetupPurrTransport(purrTransport, currentLobby);
                    }
                }
            }
            else if (networkManager.transport is PurrTransport purrTransport)
            {
                SetupPurrTransport(purrTransport, currentLobby);
            }
        }

        private void SetupPurrTransport(PurrTransport purrTransport, Lobby currentLobby)
        {
            purrTransport.roomName = currentLobby.LobbyId;
        }

        private static bool NeedToStartServer()
        {
            bool isHost = GameSettingsContainer.IsHost;
            return isHost;
        }

        public async Task StartServer()
        {
            if (NeedToStartServer())
            {
                networkManager.StartServer();

                while (networkManager.serverState != ConnectionState.Connected && !Application.exitCancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(100);
                }
            }
        }

        public async Task StartClient()
        {
            networkManager.StartClient();

            while (networkManager.clientState != ConnectionState.Connected && !Application.exitCancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100);
            }
        }
    }
}
