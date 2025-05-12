using ChessGame.Logic;
using ChessGame.Logic.Players;
using GameAndScene;
using MainCamera;
using PurrNet;
using Ui.MainMenu;
using UnityEngine;

namespace Network
{
    public class OnlineInitialization : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Game game;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private CameraController cameraController;

        [Header("Online part")]
        [SerializeField] private PlayerOnline playerOnlineWhite;
        [SerializeField] private PlayerOnline playerOnlineBlack;
        [SerializeField] private Clock clock;

        private ushort _playerId;

        private void Awake()
        {
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.onPlayerJoined += OnConnected;
            }
        }

        private void OnConnected(PlayerID player, bool isReconnect, bool asServer)
        {
            if (asServer)
            {
                InitializationAsServer(player);
            }
            else
            {
                InitializationAsClient(player);
            }
        }

        private void InitializationAsServer(PlayerID player)
        {
            if (player.id == 001)
            {
                playerOnlineWhite.GiveOwnership(player);
                clock.GiveOwnership(player);
                clock.Init(game, gameSettingsContainer.GetGameSettings(), isOffline: false);
            }
            else
            {
                playerOnlineBlack.GiveOwnership(player);
            }
        }

        private void InitializationAsClient(PlayerID player)
        {

        }

        // TODO: make disconnection
        protected override void OnDespawned()
        {
            base.OnDespawned();

            Debug.Log("");
        }

        protected override void OnOwnerDisconnected(PlayerID ownerId)
        {
            base.OnOwnerDisconnected(ownerId);

            sceneLoader.LoadMainMenu();

            if (ownerId.id == _playerId)
            {

            }
        }
    }
}