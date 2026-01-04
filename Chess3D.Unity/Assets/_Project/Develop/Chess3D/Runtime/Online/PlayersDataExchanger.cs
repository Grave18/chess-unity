using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using PurrNet;
using PurrNet.Packing;
using VContainer;

namespace Chess3D.Runtime.Online
{
    public class PlayersDataExchanger : NetworkBehaviour
    {
        private ISettingsContainer _settingsContainer;
        private ConfigContainer _configContainer;

        [Inject]
        public void Construct(ISettingsContainer settingsContainer, ConfigContainer configContainer)
        {
            _settingsContainer = settingsContainer;
            _configContainer = configContainer;
        }

        public async UniTask Load()
        {
            if (_configContainer.OnlineConfig.AuthorityType != AuthorityType.Host)
            {
                return;
            }

            await UniTask.WaitUntil(IsAllObserversAdded);
            // await UniTask.WaitUntil(() => networkManager.playerModule.localPlayerId != null);

            string serversFenPreset = _settingsContainer.S.GameSettings.CurrentFen;
            string playerName = _settingsContainer.S.GameSettings.PlayerWhite.Name;

            var connectionInfo = new ConnectionInfo
            {
                FenPreset = serversFenPreset,
                PlayerName = playerName,
            };

            PlayerID? thisPlayerID = localPlayer;
            PlayerID? otherPlayerID = networkManager.players.FirstOrDefault(otherPlayerID => otherPlayerID != thisPlayerID);
            ConnectionInfo clientConnectionInfo = await Exchange_TargetRpc(otherPlayerID.Value, connectionInfo);

            _settingsContainer.S.GameSettings.PlayerBlack.Name = clientConnectionInfo.PlayerName;
        }

        private bool IsAllObserversAdded()
        {
            return observers.Count == _configContainer.LobbyConfig.PlayersToStart;
        }

        [TargetRpc(asyncTimeoutInSec:60)]
        private Task<ConnectionInfo> Exchange_TargetRpc(PlayerID target, ConnectionInfo serverConnectionInfo)
        {
            _settingsContainer.S.GameSettings.PlayerBlack.Name = serverConnectionInfo.PlayerName;
            _settingsContainer.S.GameSettings.CurrentFen = serverConnectionInfo.FenPreset;

            var clientConnectionInfo = new ConnectionInfo
            {
                PlayerName = _settingsContainer.S.GameSettings.PlayerWhite.Name,
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