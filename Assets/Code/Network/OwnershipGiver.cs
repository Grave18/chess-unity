using Logic;
using Logic.Players;
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
                InstanceHandler.NetworkManager.onPlayerJoined += OnPlayerJoined;
                InstanceHandler.NetworkManager.onPlayerLeft += OnPlayerLeft;
            }
        }

        protected override void OnDestroy()
        {
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.onPlayerJoined -= OnPlayerJoined;
                InstanceHandler.NetworkManager.onPlayerLeft -= OnPlayerLeft;
            }
        }

        private PlayerID ServerId { get; set; }
        private void OnPlayerJoined(PlayerID player, bool isReconnect, bool asServer)
        {
            if (asServer)
            {
                SetServerId(player);
                GiveOwnershipTo(player);
            }
        }

        private void SetServerId(PlayerID player)
        {
            if (ServerId == default)
            {
                ServerId = player;
            }
        }

        private void GiveOwnershipTo(PlayerID player)
        {
            if (player.id == ServerId.id)
            {
                playerOnlineWhite.GiveOwnership(player);
                clockOnline.GiveOwnership(player);
            }
            else
            {
                playerOnlineBlack.GiveOwnership(player);
            }
        }

        private void OnPlayerLeft(PlayerID player, bool asServer)
        {
            if (asServer)
            {
                ResetServerId(player);
            }
        }

        private void ResetServerId(PlayerID player)
        {
            if (player.id == ServerId.id)
            {
                ServerId = default;
            }
        }
    }
}