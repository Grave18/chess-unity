using Network;
using PurrNet;
using Ui.MainMenu;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Ui.Game.Popups
{
    public class RematchRequestPopup : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private ChessGame.Logic.Game game;

        [Header("Ui")]
        [SerializeField] private PanelManagerInGame panelManagerInGame;
        [SerializeField] private MenuPanel endGamePopupMenuPanel;
        [SerializeField] private EndGamePopup endGamePopup;

        [Header("Buttons")]
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;

        private MenuPanel _thisMenuPanel;

        private void Awake()
        {
            _thisMenuPanel = GetComponent<MenuPanel>();
            Assert.IsNotNull(_thisMenuPanel);
        }

        private void OnEnable()
        {
            yesButton.onClick.AddListener(AcceptRematch);
            noButton.onClick.AddListener(DeclineRematch);
        }

        private void OnDisable()
        {
            yesButton.onClick.RemoveListener(AcceptRematch);
            noButton.onClick.RemoveListener(DeclineRematch);
        }

        private void AcceptRematch()
        {
            _thisMenuPanel.Hide();
            game.RestartGame();

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
            game.RestartGame();
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

            endGamePopup.DeclineRematch();
            EffectsPlayer.Instance.PlayNotifySound();
        }
    }
}
