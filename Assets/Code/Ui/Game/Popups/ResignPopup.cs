using Network;
using PurrNet;
using Ui.Common.Buttons;
using UnityEngine;

namespace Ui.Game.Popups
{
    public class ResignPopup : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private ChessGame.Logic.Game game;

        [Header("Buttons")]
        [SerializeField] private ButtonBase yesButton;

        private void OnEnable()
        {
            yesButton.OnClick += Resign;
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
            game.Resign();
        }

        [ObserversRpc]
        private void ResignOnline()
        {
            game.Resign();
        }
    }
}
