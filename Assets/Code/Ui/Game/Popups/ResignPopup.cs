using Network;
using PurrNet;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Game.Popups
{
    public class ResignPopup : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private ChessGame.Logic.Game game;

        [Header("Buttons")]
        [SerializeField] private Button yesButton;

        private void OnEnable()
        {
            yesButton.onClick.AddListener(Resign);
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
