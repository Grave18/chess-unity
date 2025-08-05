using PurrNet;
using PurrNet.Transports;
using Settings;
using Ui.InGame.ViewModels;
using UnityEngine;

namespace Network
{
    public class ConnectionHandler : NetworkBehaviour
    {
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        private void Start()
        {
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.onPlayerJoined += OnPlayerJoined;
                InstanceHandler.NetworkManager.onPlayerLeft += OnPlayerLeft;

                InstanceHandler.NetworkManager.onServerConnectionState += OnServerConnectionState;
                InstanceHandler.NetworkManager.onClientConnectionState += OnConnectionState;
            }
        }

        private void OnDisable()
        {
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.onPlayerJoined -= OnPlayerJoined;
                InstanceHandler.NetworkManager.onPlayerLeft -= OnPlayerLeft;

                InstanceHandler.NetworkManager.onServerConnectionState -= OnServerConnectionState;
                InstanceHandler.NetworkManager.onClientConnectionState -= OnConnectionState;
            }
        }

        private void OnPlayerJoined(PlayerID player, bool isReconnect, bool asServer)
        {
            if (asServer)
            {
                if (isReconnect)
                {
                    ClosePopup();
                }
            }
        }

        private static void ClosePopup()
        {
            PopupViewModel.Instance.ClosePopupToGame();
        }

        protected override void OnObserverAdded(PlayerID player)
        {
            if (player.id == 002)
            {
                SendFenPresetTo(player);
            }
        }

        private void SendFenPresetTo(PlayerID player)
        {
            string serversFenPreset = gameSettingsContainer.GetCurrentFen();
            SetFenPreset_TargetRpc(player, serversFenPreset);
        }

        [TargetRpc]
        private void SetFenPreset_TargetRpc(PlayerID target, string fenPreset)
        {
            gameSettingsContainer.SetCurrentFen(fenPreset);
        }

        private void OnPlayerLeft(PlayerID player, bool asServer)
        {
            if (asServer)
            {
                OpenPopup();
            }
        }

        private static void OpenPopup()
        {
            PopupViewModel.Instance.OpenReconnectPopup();
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