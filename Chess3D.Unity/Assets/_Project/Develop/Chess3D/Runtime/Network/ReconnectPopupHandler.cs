using PurrNet;
using Ui.InGame.ViewModels;
using UnityEngine;

namespace Chess3D.Runtime.Network
{
    public class ReconnectPopupHandler : MonoBehaviour
    {
        private void Awake()
        {
            if (InstanceHandler.NetworkManager == null)
            {
                InstanceHandler.NetworkManager.onPlayerJoined += OnPlayerJoined;
                InstanceHandler.NetworkManager.onPlayerLeft += OnPlayerLeft;
            }
        }

        private void OnDestroy()
        {
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.onPlayerJoined -= OnPlayerJoined;
                InstanceHandler.NetworkManager.onPlayerLeft -= OnPlayerLeft;
            }
        }

        private void OnPlayerJoined(PlayerID player, bool isReconnect, bool asServer)
        {
            if (asServer && isReconnect)
            {
                ClosePopup();
            }
        }

        private static void ClosePopup()
        {
            PopupViewModel.Instance.ClosePopupToGame();
        }

        private void OnPlayerLeft(PlayerID player, bool asServer)
        {
            if (IsSecondPlayer(player))
            {
                OpenReconnectPopup();
            }
        }

        private static bool IsSecondPlayer(PlayerID player)
        {
            return player.id == 002;
        }

        private static void OpenReconnectPopup()
        {
            PopupViewModel.Instance.OpenReconnectPopup();
        }
    }
}