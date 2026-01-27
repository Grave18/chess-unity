using System.Linq;
using System.Threading.Tasks;
using Chess3D.Runtime.Core.Notation;
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
        private NetworkManager _networkManager;

        private FenFromBoard _fenFromBoard;

        [Inject]
        public void Construct(ISettingsContainer settingsContainer, ConfigContainer configContainer, NetworkManager networkManager)
        {
            _settingsContainer = settingsContainer;
            _configContainer = configContainer;
            _networkManager = networkManager;
            _fenFromBoard = null;

            _networkManager.onPlayerJoined += OnPlayerJoined;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _networkManager.onPlayerJoined -= OnPlayerJoined;
        }

        public void SetFenFromBoard(FenFromBoard fenFromBoard)
        {
            _fenFromBoard = fenFromBoard;
        }

        private async void OnPlayerJoined(PlayerID player, bool isReconnect, bool asServer)
        {
            if (isReconnect)
            {
                // ConnectionInfo serverConnectionInfo = await GetConnectionInfoFromServer();
                // _settingsContainer.GameSettings.PlayerBlack.Name = serverConnectionInfo.PlayerName;
                // _settingsContainer.GameSettings.CurrentFen = _fenFromBoard.Get();
            }
        }

        public async UniTask Load()
        {
            if (_configContainer.OnlineConfig.AuthorityType == AuthorityType.Host)
            {
                await UniTask.WaitUntil(IsAllObserversAdded);
                PlayerID otherPlayerID = GetOtherPlayerID();
                ConnectionInfo clientConnectionInfo = await GetConnectionInfoFromClient(otherPlayerID);

                _settingsContainer.GameSettings.PlayerBlack.Name = clientConnectionInfo.PlayerName;
            }
            else if (_configContainer.OnlineConfig.AuthorityType == AuthorityType.Client)
            {
                await UniTask.WaitUntil(() => isSpawned);
                ConnectionInfo serverConnectionInfo = await GetConnectionInfoFromServer();
                _settingsContainer.GameSettings.PlayerBlack.Name = serverConnectionInfo.PlayerName;
                _settingsContainer.GameSettings.CurrentFen = serverConnectionInfo.FenPreset;
            }
        }

        private bool IsAllObserversAdded()
        {
            return observers.Count == _configContainer.LobbyConfig.PlayersToStart;
        }

        private PlayerID GetOtherPlayerID()
        {
            PlayerID? thisPlayerID = localPlayer;
            PlayerID? otherPlayerID = networkManager.players.FirstOrDefault(otherPlayerID => otherPlayerID != thisPlayerID);
            return otherPlayerID.Value;
        }

        [TargetRpc(asyncTimeoutInSec:60)]
        private Task<ConnectionInfo> GetConnectionInfoFromClient(PlayerID target)
        {
            var clientConnectionInfo = new ConnectionInfo
            {
                PlayerName = _settingsContainer.GameSettings.PlayerWhite.Name,
            };

            return Task.FromResult(clientConnectionInfo);
        }

        [ServerRpc(asyncTimeoutInSec:60)]
        private async Task<ConnectionInfo> GetConnectionInfoFromServer()
        {
            await UniTask.WaitUntil(() => FenValidator.IsValid(_fenFromBoard.Get(), out _));

            string serversFenPreset = _fenFromBoard.Get();
            string playerName = _settingsContainer.GameSettings.PlayerWhite.Name;

            var connectionInfo = new ConnectionInfo
            {
                FenPreset = serversFenPreset,
                PlayerName = playerName,
            };

            return connectionInfo;
        }
    }

    internal struct ConnectionInfo : IPackedAuto
    {
        public string FenPreset;
        public string PlayerName;
    }
}