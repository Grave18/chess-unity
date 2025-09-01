using System.Collections;
using PurrNet;
using PurrNet.Logging;
using PurrNet.Transports;
using UnityEngine;
using UtilsCommon;

#if UTP_LOBBYRELAY
using PurrNet.UTP;
using Unity.Services.Relay.Models;
#endif

namespace LobbyManagement
{
    public class ConnectionStarter : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;

        private LobbyDataHolder _lobbyDataHolder;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            _lobbyDataHolder = FindFirstObjectByType<LobbyDataHolder>();

            if(!_lobbyDataHolder)
            {
                PurrLogger.LogError($"Failed to get {nameof(LobbyDataHolder)} component.", this);
            }
        }

        private void Start()
        {
            if (IsReferencesNotValid())
            {
                return;
            }

            SetupPurrTransport();
            SetupUtpTransport();

            if(CanStartServer())
            {
                StartServer();
            }

            StartClient().RunCoroutine();
        }

        private bool IsReferencesNotValid()
        {
            if (!networkManager)
            {
                PurrLogger.LogError($"Failed to start connection. {nameof(NetworkManager)} is null!", this);
                return true;
            }

            if (!_lobbyDataHolder)
            {
                PurrLogger.LogError($"Failed to start connection. {nameof(LobbyDataHolder)} is null!", this);
                return true;
            }

            if (!_lobbyDataHolder.CurrentLobby.IsValid)
            {
                PurrLogger.Log("Offline game. Can't start connection. Destroying multiplayer part", this);
                // Destroy(gameObject);
                return true;
            }

            return false;
        }

        private void SetupPurrTransport()
        {
            if(networkManager.transport is PurrTransport transport) {
                transport.roomName = _lobbyDataHolder.CurrentLobby.LobbyId;
            }
        }

        private void SetupUtpTransport()
        {
#if UTP_LOBBYRELAY
            else if(_networkManager.transport is UTPTransport) {
                if(_lobbyDataHolder.CurrentLobby.IsOwner) {
                    (_networkManager.transport as UTPTransport).InitializeRelayServer((Allocation)_lobbyDataHolder.CurrentLobby.ServerObject);
                }
                (_networkManager.transport as UTPTransport).InitializeRelayClient(_lobbyDataHolder.CurrentLobby.Properties["JoinCode"]);
            }
#else
            //P2P Connection, receive IP/Port from server
#endif
        }

        private bool CanStartServer()
        {
            return _lobbyDataHolder.CurrentLobby.IsOwner;
        }

        private void StartServer()
        {
            networkManager.StartServer();
        }

        private IEnumerator StartClient()
        {
            yield return new WaitForSeconds(1f);
            networkManager.StartClient();
        }
    }
}
