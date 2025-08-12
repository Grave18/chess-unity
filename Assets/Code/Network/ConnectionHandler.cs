using System.Threading.Tasks;
using PurrNet;
using PurrNet.Packing;
using Settings;
using Ui.InGame.ViewModels;
using UnityEngine;
using UtilsCommon;

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
                ExchangeDataBetweenPlayers(player);
            }
        }

        private void ExchangeDataBetweenPlayers(PlayerID connectedPlayer)
        {
            string serversFenPreset = gameSettingsContainer.GetCurrentFen();
            string playerName = gameSettingsContainer.GameSettings.Player1Settings.Name;
            var connectionInfo = new ConnectionInfo
            {
                FenPreset = serversFenPreset,
                PlayerName = playerName,
            };

            ExchangeDataBetweenPlayers_TargetRpc(connectedPlayer, connectionInfo)
                .ContinueOnMainThread(clientConnectionInfoTask =>
                {
                    gameSettingsContainer.SetPlayer2Name(clientConnectionInfoTask.Result.PlayerName);
                });
        }

        [TargetRpc]
        private Task<ConnectionInfo> ExchangeDataBetweenPlayers_TargetRpc(PlayerID target, ConnectionInfo serverConnectionInfo)
        {
            gameSettingsContainer.SetPlayer2Name(serverConnectionInfo.PlayerName);
            gameSettingsContainer.SetCurrentFen(serverConnectionInfo.FenPreset);

            var clientConnectionInfo = new ConnectionInfo
            {
                PlayerName = gameSettingsContainer.GameSettings.Player1Settings.Name,
            };

            return Task.FromResult(clientConnectionInfo);
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

    internal struct ConnectionInfo : IPackedAuto
    {
        public string FenPreset;
        public string PlayerName;
    }
}