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
    public class DrawPopup : NetworkBehaviour
    {
        [Header("UI")]
        [SerializeField] private PanelManagerInGame panelManagerInGame;
        [SerializeField] private MenuPanel requestDrawPopup;
        [SerializeField] private TMP_Text text;

        [Header("Buttons")]
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;
        [SerializeField] private TMP_Text noButtonText;

        [Header("Texts")]
        [SerializeField] private string initialText = "Do you want to offer draw?";
        [SerializeField] private string offeredText = "Draw Offered";
        [SerializeField] private string drawDeclinedText2 = "Draw Declined. This window will be closed";

        [Header("Settings")]
        [SerializeField] private float timeToCloseAfterDeclineSec = 3f;

        private MenuPanel _thisMenuPanel;

        private void Awake()
        {
            _thisMenuPanel = GetComponentInParent<MenuPanel>();
            Assert.IsNotNull(_thisMenuPanel);
        }

        private void OnEnable()
        {
            yesButton.onClick.AddListener(OfferDraw);
            RestoreState();
        }

        private void RestoreState()
        {
            text.text = initialText;
            yesButton.interactable = true;
            noButtonText.text = "No";
        }

        private void OnDisable()
        {
            yesButton.onClick.RemoveListener(OfferDraw);
        }

        private void OfferDraw()
        {
            yesButton.interactable = false;
            noButtonText.text = "Close";
            text.text = offeredText;

            PlayerID otherPlayerID = OnlineInstanceHandler.OtherPlayerID;
            OfferDrawTarget(otherPlayerID);
        }

        [ObserversRpc]
        private void OfferDrawTarget(PlayerID playerId)
        {
            if(playerId.id != localPlayer?.id)
            {
                return;
            }

            requestDrawPopup.Show();
            EffectsPlayer.Instance.PlayNotifySound();
        }

        public void DeclineDrawByRequestPopup()
        {
            text.text = drawDeclinedText2;
            EffectsPlayer.Instance.PlayNotifySound();

            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(CloseThisPanelRoutine());
            }
        }

        private IEnumerator CloseThisPanelRoutine()
        {
            yield return new WaitForSeconds(timeToCloseAfterDeclineSec);

            _thisMenuPanel.Hide();
            panelManagerInGame.ShowMenuButtonPanel();
        }
    }
}
