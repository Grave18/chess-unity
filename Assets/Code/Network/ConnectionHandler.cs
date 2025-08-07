using PurrNet;
using Settings;
using Ui.InGame.ViewModels;
using UnityEngine;

namespace Network
{
    public class ConnectionHandler : NetworkBehaviour
    {
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        private void Awake()
        {
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.onPlayerJoined += OnPlayerJoined;
                InstanceHandler.NetworkManager.onPlayerLeft += OnPlayerLeft;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.onPlayerJoined -= OnPlayerJoined;
                InstanceHandler.NetworkManager.onPlayerLeft -= OnPlayerLeft;
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
            SetFenPresetTo_TargetRpc(player, serversFenPreset);
        }

        [TargetRpc]
        private void SetFenPresetTo_TargetRpc(PlayerID target, string fenPreset)
        {
            gameSettingsContainer.SetCurrentFen(fenPreset);
        }

        private void OnPlayerLeft(PlayerID player, bool asServer)
        {
            if (asServer)
            {
                return;
            }

            if (player.id == 002)
            {
                OpenReconnectPopup();
            }
        }

        private static void OpenReconnectPopup()
        {
            PopupViewModel.Instance.OpenReconnectPopup();
        }
    }
}