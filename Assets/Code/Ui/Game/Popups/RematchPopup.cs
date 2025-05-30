using System.Collections;
using Network;
using PurrNet;
using TMPro;
using Ui.MainMenu;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

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
        [SerializeField] private Button yesButton;
        [SerializeField] private TMP_Text noButtonText;

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
            yesButton.onClick.AddListener(Rematch);

            RestoreState();
        }

        private void RestoreState()
        {
            text.text = initialText;
            yesButton.interactable = true;
            noButtonText.text = noText;
        }

        private void OnDisable()
        {
            yesButton.onClick.RemoveListener(Rematch);
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
            yesButton.interactable = false;
            text.text = rematchRequestedText;
            noButtonText.text = closeText;

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
            yesButton.interactable = true;

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