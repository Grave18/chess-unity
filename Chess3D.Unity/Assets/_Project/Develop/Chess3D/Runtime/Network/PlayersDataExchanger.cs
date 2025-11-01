using System.Threading.Tasks;
using Chess3D.Runtime.Settings;
using PurrNet;
using PurrNet.Packing;
using UnityEngine;

namespace Chess3D.Runtime.Network
{
    public class PlayersDataExchanger : NetworkBehaviour
    {
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        public int ObserversCount => observers.Count;

        public async Task Exchange()
        {
            PlayerID connectedPlayer = OnlineInstanceHandler.OtherPlayerID;
            string serversFenPreset = gameSettingsContainer.GetCurrentFen();
            string playerName = gameSettingsContainer.GameSettings.Player1Settings.Name;

            var connectionInfo = new ConnectionInfo
            {
                FenPreset = serversFenPreset,
                PlayerName = playerName,
            };

            ConnectionInfo clientConnectionInfo = await Exchange_TargetRpc(connectedPlayer, connectionInfo);

            gameSettingsContainer.SetPlayer2Name(clientConnectionInfo.PlayerName);
        }

        [TargetRpc(asyncTimeoutInSec:60)]
        private Task<ConnectionInfo> Exchange_TargetRpc(PlayerID target, ConnectionInfo serverConnectionInfo)
        {
            gameSettingsContainer.SetPlayer2Name(serverConnectionInfo.PlayerName);
            gameSettingsContainer.SetCurrentFen(serverConnectionInfo.FenPreset);

            var clientConnectionInfo = new ConnectionInfo
            {
                PlayerName = gameSettingsContainer.GameSettings.Player1Settings.Name,
            };

            return Task.FromResult(clientConnectionInfo);
        }
    }

    internal struct ConnectionInfo : IPackedAuto
    {
        public string FenPreset;
        public string PlayerName;
    }
}