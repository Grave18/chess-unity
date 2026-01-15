using System;
using PurrNet;
using UnityEngine.Scripting;
using PopupViewModel = Chess3D.Runtime.Core.UI.ViewModels.PopupViewModel;

namespace Chess3D.Runtime.Online
{
    [Preserve]
    public class ReconnectPopupHandler : IDisposable
    {
        private readonly NetworkManager _networkManager;
        private PopupViewModel _popupViewModel;

        private ReconnectPopupHandler(NetworkManager networkManager)
        {
            _networkManager = networkManager;

            _networkManager.onPlayerJoined += OnPlayerJoined;
            _networkManager.onPlayerLeft += OnPlayerLeft;
        }

        public void Dispose()
        {
            _networkManager.onPlayerJoined -= OnPlayerJoined;
            _networkManager.onPlayerLeft -= OnPlayerLeft;
        }

        public void SetPopupViewModel(PopupViewModel popupViewModel)
        {
            _popupViewModel = popupViewModel;
        }

        private void OnPlayerJoined(PlayerID player, bool isReconnect, bool asServer)
        {
            if (asServer && isReconnect)
            {
                ClosePopup();
            }
        }

        private void ClosePopup()
        {
            _popupViewModel.ClosePopupToGame();
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

        private void OpenReconnectPopup()
        {
            _popupViewModel.OpenReconnectPopup();
        }
    }
}