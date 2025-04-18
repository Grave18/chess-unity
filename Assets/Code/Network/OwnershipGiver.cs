using Logic;
using Logic.Players;
using PurrNet;
using UnityEngine;

public class OwnershipGiver : NetworkBehaviour
{
    [SerializeField] private PlayerOnline playerOnlineWhite;
    [SerializeField] private PlayerOnline playerOnlineBlack;
    [SerializeField] private Clock clock;

    private void Awake()
    {
        if (InstanceHandler.NetworkManager != null)
        {
            InstanceHandler.NetworkManager.onPlayerJoined += OnConnected;
            // InstanceHandler.NetworkManager.onPlayerJoinedScene += OnJoinedScene;
        }
    }

    private void OnConnected(PlayerID player, bool isReconnect, bool asServer)
    {
        if (asServer)
        {
            if (player.id == 001)
            {
                playerOnlineWhite.GiveOwnership(player);
                clock.GiveOwnership(player);
            }
            else
            {
                playerOnlineBlack.GiveOwnership(player);
            }
        }
    }

    private void OnJoinedScene(PlayerID player, SceneID scene, bool asServer)
    {
        if (asServer)
        {
            if (player.id == 001)
            {
                playerOnlineWhite.GiveOwnership(player);
            }
            else
            {
                playerOnlineBlack.GiveOwnership(player);
            }
        }
    }
}