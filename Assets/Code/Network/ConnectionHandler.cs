using PurrNet;
using PurrNet.Transports;
using Ui.InGame.ViewModels;
using UnityEngine;

namespace Network
{
    public class ConnectionHandler : NetworkBehaviour
    {
        private void Start()
        {
            InstanceHandler.NetworkManager.onServerConnectionState += OnServerConnectionState;
            InstanceHandler.NetworkManager.onClientConnectionState += OnConnectionState;
            InstanceHandler.NetworkManager.onPlayerJoined += OnPlayerJoined;
            InstanceHandler.NetworkManager.onPlayerLeft += OnPlayerLeft;
        }

        private void OnDisable()
        {
            InstanceHandler.NetworkManager.onServerConnectionState -= OnServerConnectionState;
            InstanceHandler.NetworkManager.onClientConnectionState -= OnConnectionState;
            InstanceHandler.NetworkManager.onPlayerJoined -= OnPlayerJoined;
            InstanceHandler.NetworkManager.onPlayerLeft -= OnPlayerLeft;
        }

        private void OnPlayerJoined(PlayerID player, bool isReconnect, bool asServer)
        {
            if (asServer)
            {
                Debug.Log($"Player {player} joined. IsReconnect: {isReconnect}");

                if (isReconnect)
                {
                    PopupViewModel.Instance.ClosePopupToGame();
                }
            }
        }

        private void OnPlayerLeft(PlayerID player, bool asServer)
        {
            if (asServer)
            {
                Debug.Log($"Player {player} left. Wait for reconnect");
                PopupViewModel.Instance.OpenReconnectPopup();
            }
        }

        private void OnServerConnectionState(ConnectionState serverState)
        {

        }

        private void OnConnectionState(ConnectionState clientState)
        {
            if (clientState == ConnectionState.Connecting)
            {

            }
            else if (clientState == ConnectionState.Connected)
            {

            }
            else if (clientState == ConnectionState.Disconnecting)
            {

            }
            else if (clientState == ConnectionState.Disconnected)
            {

            }
        }
    }
}