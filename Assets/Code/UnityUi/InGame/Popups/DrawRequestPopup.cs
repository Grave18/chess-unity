using Network;
using PurrNet;
using UnityEngine;
using UnityEngine.Assertions;
using UnityUi.Common.Buttons;
using UnityUi.Menu;

namespace UnityUi.InGame.Popups
{
    // This panel can be opened only in online mode
    public class DrawRequestPopup : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Logic.Game game;

        [Header("Ui")]
        [SerializeField] private DrawPopup drawPopup;

        [Header("Buttons")]
        [SerializeField] private ButtonBase yesButton;
        [SerializeField] private ButtonBase noButton;

        private MenuPanel _thisMenuPanel;

        private void Awake()
        {
            _thisMenuPanel = GetComponent<MenuPanel>();
            Assert.IsNotNull(_thisMenuPanel);
        }

        private void OnEnable()
        {
            yesButton.OnClick += AcceptDraw;
            noButton.OnClick += DeclineDraw;
        }

        private void OnDisable()
        {
            yesButton.OnClick -= AcceptDraw;
            noButton.OnClick -= DeclineDraw;
        }

        private void AcceptDraw()
        {
            _thisMenuPanel.Hide();
            // game.DrawByAgreement();

            AcceptDrawTarget(OnlineInstanceHandler.OtherPlayerID);
        }

        [ObserversRpc]
        private void AcceptDrawTarget(PlayerID playerId)
        {
            if(playerId.id != localPlayer?.id)
            {
                return;
            }

            // game.DrawByAgreement();
            drawPopup.AcceptDrawByRequestPopup();
        }

        private void DeclineDraw()
        {
            _thisMenuPanel.Hide();
            DeclineDrawTarget(OnlineInstanceHandler.OtherPlayerID);
        }

        [ObserversRpc]
        private void DeclineDrawTarget(PlayerID playerId)
        {
            if(playerId.id != localPlayer?.id)
            {
                return;
            }

            drawPopup.DeclineDrawByRequestPopup();
        }
    }
}
