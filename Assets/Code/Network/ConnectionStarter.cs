using System.Collections;
using LobbyManagement;
using PurrNet;
using PurrNet.Logging;
using PurrNet.Transports;
using Settings;
using UnityEngine;
using UtilsCommon;

namespace Network
{
    public class ConnectionStarter : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        private LobbyDataHolder _lobbyDataHolder;

        private void Awake()
        {
            _lobbyDataHolder = FindFirstObjectByType<LobbyDataHolder>();
        }

        private void Start()
        {
            CheckReferences();

            SetupTransports();

            if(CanStartServer())
            {
                StartServer();
            }

            StartClient().RunCoroutine();
        }

        private void CheckReferences()
        {
            if (!networkManager)
            {
                PurrLogger.LogError($"Failed to start connection. {nameof(NetworkManager)} is null!", this);
            }

            if (!_lobbyDataHolder)
            {
                PurrLogger.LogError($"Failed to start connection. {nameof(LobbyDataHolder)} is null!", this);
            }

            if (!_lobbyDataHolder.CurrentLobby.IsValid)
            {
                PurrLogger.Log("Lobby is not valid.", this);
            }
        }

        private void SetupTransports()
        {
            if(networkManager.transport is CompositeTransport composite)
            {
                foreach (GenericTransport transport in composite.transports)
                {
                    if(transport is PurrTransport purrTransport)
                    {
                        SetupPurrTransport(purrTransport);
                    }
                }
            }
            else if (networkManager.transport is PurrTransport purrTransport)
            {
                SetupPurrTransport(purrTransport);
            }
        }

        private void SetupPurrTransport(PurrTransport purrTransport)
        {
            purrTransport.roomName = _lobbyDataHolder.CurrentLobby.LobbyId;
        }

        private bool CanStartServer()
        {
            bool isHost = GameSettingsContainer.IsHost;
            return isHost;
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
