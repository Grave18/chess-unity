using Chess3D.Runtime.Menu.LobbyManagement;
using Cysharp.Threading.Tasks;
using PurrNet;
using PurrNet.Transports;
using VContainer;

namespace Chess3D.Runtime.Online
{
    public class ConnectionStarter
    {
        private NetworkManager _networkManager;
        private PurrTransport _purrTransport;
        private LocalTransport _localTransport;
        private UDPTransport _udpTransport;
        private ConfigContainer _configContainer;
        private Lobby _currentLobby;

        [Inject]
        public void Construct(NetworkManager networkManager, PurrTransport purrTransport, LocalTransport localTransport,
            UDPTransport udpTransport, ConfigContainer configContainer, LobbyDataHolder lobbyDataHolder)
        {
            _networkManager = networkManager;
            _purrTransport = purrTransport;
            _localTransport = localTransport;
            _udpTransport = udpTransport;
            _configContainer = configContainer;
            _currentLobby = lobbyDataHolder.CurrentLobby;
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
                _networkManager.transport = _udpTransport;
            }
            else
            {
                _networkManager.transport = _purrTransport;
                _purrTransport.roomName = _currentLobby.LobbyId;
            }
        }

        public async UniTask StartHostOrClient()
        {
            if (_configContainer.OnlineConfig.AuthorityType == AuthorityType.Host)
            {
                _networkManager.StartServer();
                await UniTask.WaitUntil(() => _networkManager.serverState == ConnectionState.Connected);
            }

            _networkManager.StartClient();
            await UniTask.WaitUntil(() => _networkManager.clientState == ConnectionState.Connected);
        }
    }
}
