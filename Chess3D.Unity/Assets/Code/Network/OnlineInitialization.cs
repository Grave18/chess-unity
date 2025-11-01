using Cysharp.Threading.Tasks;
using LobbyManagement;
using Logic;
using Logic.Players;
using PurrNet;
using Settings;
using UnityEngine;
using UtilsCommon.Singleton;

namespace Network
{
    public class OnlineInitialization : SingletonBehaviour<OnlineInitialization>
    {
        [Header("References")]
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private ConnectionStarter connectionStarter;
        [SerializeField] private PlayersDataExchanger playersDataExchanger;
        [SerializeField] private OnlineSceneLoader onlineSceneLoader;

        [Header("Owned objects")]
        [SerializeField] private PlayerOnline playerOnlineWhite;
        [SerializeField] private PlayerOnline playerOnlineBlack;
        [SerializeField] private ClockOnline clockOnline;

        private LobbyDataHolder _lobbyDataHolder;

        public async UniTask Init()
        {
            GetLobbyDataHolder();
            await SetupAndStartNetworking();
            await InitOnlineInstanceHandler();

            if (GameSettingsContainer.IsHost)
            {
                await GiveOwnership();
                await ExchangeDataBetweenPlayers();
            }
        }

        public async UniTask LoadGame()
        {
            await onlineSceneLoader.LoadGame();
        }

        private void GetLobbyDataHolder()
        {
            _lobbyDataHolder = FindFirstObjectByType<LobbyDataHolder>();
        }

        private async UniTask SetupAndStartNetworking()
        {
            connectionStarter.SetupTransports(_lobbyDataHolder.CurrentLobby);
            await connectionStarter.StartServerIfHost();
            await connectionStarter.StartClient();
        }

        private static async UniTask InitOnlineInstanceHandler()
        {
            await UniTask.WaitWhile(() => InstanceHandler.NetworkManager?.localPlayer.id.value == 0);

            if (InstanceHandler.NetworkManager != null)
            {
                OnlineInstanceHandler.Init(InstanceHandler.NetworkManager.localPlayer);
            }
        }

        private async UniTask GiveOwnership()
        {
            await UniTask.WaitWhile(() => networkManager.playerCount < 2);

            SetupHostOwnership(OnlineInstanceHandler.ThisPlayerID);
            SetupClientOwnership(OnlineInstanceHandler.OtherPlayerID);
        }

        private void SetupHostOwnership(PlayerID player)
        {
            playerOnlineWhite.GiveOwnership(player);
            clockOnline.GiveOwnership(player);
        }

        private void SetupClientOwnership(PlayerID player)
        {
            playerOnlineBlack.GiveOwnership(player);
        }

        private async UniTask ExchangeDataBetweenPlayers()
        {
            await UniTask.WaitWhile(() => playersDataExchanger.ObserversCount < 2);

            await playersDataExchanger.Exchange();
        }
    }
}