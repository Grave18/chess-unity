using System.Collections;
using Network;
using PurrNet;
using TMPro;
using Ui.Common.Buttons;
using Ui.MainMenu;
using UnityEngine;
using UnityEngine.Assertions;

namespace Ui.Game.Popups
{
    public class RematchPopup : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private ChessGame.Logic.Game game;

        [Header("UI")]
        [SerializeField] private PanelManagerInGame panelManagerInGame;
        [SerializeField] private MenuPanel rematchRequestPopup;
        [SerializeField] private TMP_Text text;

        [Header("Buttons")]
        [SerializeField] private ButtonBase yesButton;
        [SerializeField] private ButtonBase noButton;

        [Header("Text")]
        [SerializeField] private string initialText = "Do you want to rematch?";
        [SerializeField] private string rematchRequestedText = "Rematch requested";
        [SerializeField] private string rematchDeclinedText = "Rematch declined. This window will be closed";

        [SerializeField] private string noText = "No";
        [SerializeField] private string closeText = "Close";

        private MenuPanel _thisMenuPanel;

        private void Awake()
        {
            _thisMenuPanel = GetComponentInParent<MenuPanel>();
            Assert.IsNotNull(_thisMenuPanel);
        }

        private void OnEnable()
        {
            yesButton.OnClick += Rematch;

            RestoreState();
        }

        private void RestoreState()
        {
            text.text = initialText;
            yesButton.Interactable = true;
            noButton.Text = noText;
        }

        private void OnDisable()
        {
            yesButton.OnClick -= Rematch;
        }

        private void Rematch()
        {
            if (OnlineInstanceHandler.IsOffline)
            {
                RematchOffline();
            }
            else
            {
                RematchOnline();
            }
        }

        private void RematchOffline()
        {
            _thisMenuPanel.Hide();
            game.RestartGame();
        }

        private void RematchOnline()
        {
            yesButton.Interactable = false;
            text.text = rematchRequestedText;
            noButton.Text = closeText;

            PlayerID otherPlayerID = OnlineInstanceHandler.OtherPlayerID;
            RequestRematchTarget(otherPlayerID);
        }

        [ObserversRpc]
        private void RequestRematchTarget(PlayerID playerId)
        {
            if(playerId.id != localPlayer?.id)
            {
                return;
            }

            rematchRequestPopup.Show();
            EffectsPlayer.Instance.PlayNotifySound();
        }

        public void AcceptRematchByRequestPopup()
        {
            panelManagerInGame.ShowMenuButtonPanel();
        }

        public void DeclineRematchByRequestPopup()
        {
            text.text = rematchDeclinedText;
            yesButton.Interactable = true;

            CloseThisPanelAfterTime();
        }

        private void CloseThisPanelAfterTime()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(CloseThisPanelRoutine());
            }

            return;

            IEnumerator CloseThisPanelRoutine()
            {
                yield return new WaitForSeconds(3f);

                panelManagerInGame.ShowMenuButtonPanel();
            }
        }
    }
}