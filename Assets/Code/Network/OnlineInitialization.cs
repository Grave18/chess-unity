using System;
using System.Threading.Tasks;
using LobbyManagement;
using Logic;
using Logic.Players;
using PurrNet;
using Settings;
using UnityEngine;

namespace Network
{
    public class OnlineInitialization : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private ConnectionStarter connectionStarter;
        [SerializeField] private PlayersDataExchanger playersDataExchanger;
        [SerializeField] private GameSceneLoader gameSceneLoader;

        [Header("Online part")]
        [SerializeField] private PlayerOnline playerOnlineWhite;
        [SerializeField] private PlayerOnline playerOnlineBlack;
        [SerializeField] private ClockOnline clockOnline;

        private LobbyDataHolder _lobbyDataHolder;

        private async void Awake()
        {
            try
            {
                await Initialize();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private async Task Initialize()
        {
            GetLobbyDataHolder();
            await SetupAndStartNetworking();
            await InitOnlineInstanceHandler();

            if (GameSettingsContainer.IsHost)
            {
                await GiveOwnership();
                await ExchangeDataBetweenPlayers();
                gameSceneLoader.Load();
            }
        }

        private void GetLobbyDataHolder()
        {
            _lobbyDataHolder = FindFirstObjectByType<LobbyDataHolder>();
        }

        private async Task SetupAndStartNetworking()
        {
            connectionStarter.SetupTransports(_lobbyDataHolder.CurrentLobby);
            await connectionStarter.StartServer();
            await connectionStarter.StartClient();
        }

        private static async Task InitOnlineInstanceHandler()
        {
            while (InstanceHandler.NetworkManager?.localPlayer.id.value is 0 && !Application.exitCancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100);
            }

            if (InstanceHandler.NetworkManager != null)
            {
                OnlineInstanceHandler.Init(InstanceHandler.NetworkManager.localPlayer);
            }
        }

        private async Task GiveOwnership()
        {
            while (networkManager.playerCount < 2 && !Application.exitCancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100);
            }

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

        private async Task ExchangeDataBetweenPlayers()
        {
            while (playersDataExchanger.ObserversCount < 2 && !Application.exitCancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100);
            }

            await playersDataExchanger.Exchange();
        }
    }
}