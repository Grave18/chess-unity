using ChessGame.Logic;
using ChessGame.Logic.Players;
using PurrNet;
using UnityEngine;

namespace Network
{
    public class OwnershipGiver : NetworkBehaviour
    {
        [Header("Online part")]
        [SerializeField] private PlayerOnline playerOnlineWhite;
        [SerializeField] private PlayerOnline playerOnlineBlack;
        [SerializeField] private ClockOnline clockOnline;

        private void Awake()
        {
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.onPlayerJoined += OnConnected;
            }
        }

        protected override void OnDestroy()
        {
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.onPlayerJoined -= OnConnected;
            }
        }

        private void OnConnected(PlayerID player, bool isReconnect, bool asServer)
        {
            if (asServer)
            {
                InitializationAsServer(player);
            }
        }

        private void InitializationAsServer(PlayerID player)
        {
            if (player.id == 001)
            {
                playerOnlineWhite.GiveOwnership(player);
                clockOnline.GiveOwnership(player);
            }
            else
            {
                playerOnlineBlack.GiveOwnership(player);
            }
        }
    }
}