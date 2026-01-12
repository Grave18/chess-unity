using System;
using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Logic.Players;
using PurrNet;
using VContainer;
using VContainer.Unity;

namespace Chess3D.Runtime.Online
{
    [UnityEngine.Scripting.Preserve]
    public class OwnershipGiver : IInitializable, IDisposable
    {
        private readonly PlayerOnline _playerOnlineWhite;
        private readonly PlayerOnline _playerOnlineBlack;

        public OwnershipGiver([Key(PieceColor.White)] PlayerOnline playerOnlineWhite,
            [Key(PieceColor.Black)] PlayerOnline playerOnlineBlack)
        {
            _playerOnlineWhite = playerOnlineWhite;
            _playerOnlineBlack = playerOnlineBlack;

            InstanceHandler.NetworkManager.onPlayerJoined += OnPlayerJoined;
        }

        public void Dispose()
        {
            InstanceHandler.NetworkManager.onPlayerJoined -= OnPlayerJoined;
        }

        public void Initialize()
        {
            // For create instantly in container build time
        }

        private void OnPlayerJoined(PlayerID player, bool isReconnect, bool asServer)
        {
            if (asServer)
            {
                GiveOwnershipTo(player);
            }
        }

        private void GiveOwnershipTo(PlayerID player)
        {
            if (player.id == 001)
            {
                _playerOnlineWhite.GiveOwnership(player);
            }
            else
            {
                _playerOnlineBlack.GiveOwnership(player);
            }
        }
    }
}