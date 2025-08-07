using Network;
using PurrNet;
using Sound;
using Ui.Common.Buttons;
using Ui.MainMenu;
using UnityEngine;
using UnityEngine.Assertions;

namespace Ui.Game.Popups
{
    public class RematchRequestPopup : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Logic.Game game;

        [Header("Ui")]
        [SerializeField] private PanelManagerInGame panelManagerInGame;
        [SerializeField] private MenuPanel endGamePopupMenuPanel;
        [SerializeField] private RematchPopup rematchPopup;

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
            yesButton.OnClick += AcceptRematch;
            noButton.OnClick += DeclineRematch;
        }

        private void OnDisable()
        {
            yesButton.OnClick -= AcceptRematch;
            noButton.OnClick -= DeclineRematch;
        }

        private void AcceptRematch()
        {
            _thisMenuPanel.Hide();
            game.Rematch();

            AcceptRematchTarget(OnlineInstanceHandler.OtherPlayerID);
        }

        [ObserversRpc]
        private void AcceptRematchTarget(PlayerID playerId)
        {
            if(playerId.id != localPlayer?.id)
            {
                return;
            }

            endGamePopupMenuPanel.Hide();
            rematchPopup.AcceptRematchByRequestPopup();
            game.Rematch();
        }

        private void DeclineRematch()
        {
            _thisMenuPanel.Hide();
            DeclineRematchTarget(OnlineInstanceHandler.OtherPlayerID);
        }

        [ObserversRpc]
        private void DeclineRematchTarget(PlayerID playerId)
        {
            if(playerId.id != localPlayer?.id)
            {
                return;
            }

            rematchPopup.DeclineRematchByRequestPopup();
            EffectsPlayer.Instance.PlayNotifySound();
        }
    }
}
