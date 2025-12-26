using Chess3D.Runtime.Menu.LobbyManagement;
using Cysharp.Threading.Tasks;
using PurrNet;
using PurrNet.Transports;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Online
{
    public class ConnectionStarter : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private PurrTransport purrTransport;
        [SerializeField] private LocalTransport localTransport;
        [SerializeField] private UDPTransport udpTransport;

        private ConfigContainer _configContainer;
        private Lobby _currentLobby;

        [Inject]
        public void Construct(ConfigContainer configContainer, Lobby currentLobby)
        {
            _configContainer = configContainer;
            _currentLobby = currentLobby;
        }

        public async UniTask Load()
        {
            SetupTransports();
            await StartHostOrClient();
        }

        public void SetupTransports()
        {
            if (_configContainer.OnlineConfig.IsLocal)
            {
                networkManager.transport = udpTransport;
            }
            else
            {
                networkManager.transport = purrTransport;
                purrTransport.roomName = _currentLobby.LobbyId;
            }
        }

        public async UniTask StartHostOrClient()
        {
            if (_configContainer.OnlineConfig.AuthorityType == AuthorityType.Host)
            {
                networkManager.StartServer();
                await UniTask.WaitUntil(() => networkManager.serverState == ConnectionState.Connected);
            }

            networkManager.StartClient();
            await UniTask.WaitUntil(() => networkManager.clientState == ConnectionState.Connected);
        }
    }
}
