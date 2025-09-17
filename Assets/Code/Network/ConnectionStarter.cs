using Cysharp.Threading.Tasks;
using LobbyManagement;
using PurrNet;
using PurrNet.Transports;
using Settings;
using UnityEngine;

namespace Network
{
    public class ConnectionStarter : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private PurrTransport purrTransport;
        [SerializeField] private LocalTransport localTransport;
        [SerializeField] private UDPTransport udpTransport;

        public void SetupTransports(Lobby currentLobby)
        {
            if (GameSettingsContainer.IsLocal)
            {
                networkManager.transport = udpTransport;
            }
            else
            {
                networkManager.transport = purrTransport;
                purrTransport.roomName = currentLobby.LobbyId;
            }
        }

        public async UniTask StartServerIfHost()
        {
            if (GameSettingsContainer.IsHost)
            {
                networkManager.StartServer();

                await UniTask.WaitUntil(() => networkManager.serverState == ConnectionState.Connected);
            }
        }

        public async UniTask StartClient()
        {
            networkManager.StartClient();

            await UniTask.WaitUntil(() => networkManager.clientState == ConnectionState.Connected);
        }
    }
}
