using Chess3D.Runtime.Menu.LobbyManagement;
using Cysharp.Threading.Tasks;
using PurrNet;
using PurrNet.Transports;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Online
{
    [Preserve]
    public class ConnectionStarter
    {
        private readonly NetworkManager _networkManager;
        private readonly PurrTransport _purrTransport;
        private LocalTransport _localTransport;
        private readonly UDPTransport _udpTransport;
        private readonly ConfigContainer _configContainer;
        private readonly Lobby _currentLobby;

        public ConnectionStarter(NetworkManager networkManager, PurrTransport purrTransport, LocalTransport localTransport,
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

        private void SetupTransports()
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

        private async UniTask StartHostOrClient()
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
