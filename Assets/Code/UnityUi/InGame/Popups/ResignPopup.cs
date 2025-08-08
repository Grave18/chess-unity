using Network;
using PurrNet;
using UnityEngine;
using UnityUi.Common.Buttons;

namespace UnityUi.InGame.Popups
{
    public class ResignPopup : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Logic.Game game;

        [Header("Buttons")]
        [SerializeField] private ButtonBase yesButton;

        private void OnEnable()
        {
            yesButton.OnClick += Resign;
        }

        private void OnDisable()
        {
            yesButton.OnClick -= Resign;
        }

        private void Resign()
        {
            if (OnlineInstanceHandler.IsOffline)
            {
                ResignOffline();
            }
            else
            {
                ResignOnline();
            }
        }

        private void ResignOffline()
        {
            // game.Resign();
        }

        [ObserversRpc]
        private void ResignOnline()
        {
            // game.Resign();
        }
    }
}
