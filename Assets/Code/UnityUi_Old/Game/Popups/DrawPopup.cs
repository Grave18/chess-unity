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
    public class DrawPopup : NetworkBehaviour
    {
        [Header("UI")]
        [SerializeField] private PanelManagerInGame panelManagerInGame;
        [SerializeField] private MenuPanel requestDrawPopup;
        [SerializeField] private TMP_Text text;

        [Header("Buttons")]
        [SerializeField] private ButtonBase yesButton;
        [SerializeField] private ButtonBase noButton;

        [Header("Texts")]
        [SerializeField] private string initialText = "Do you want to offer draw?";
        [SerializeField] private string offeredText = "Draw Offered";
        [SerializeField] private string drawDeclinedText2 = "Draw Declined. This window will be closed";

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
            yesButton.OnClick += OfferDraw;
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
            yesButton.OnClick -= OfferDraw;
        }

        private void OfferDraw()
        {
            yesButton.Interactable = false;
            noButton.Text = closeText;
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

        public void AcceptDrawByRequestPopup()
        {
            panelManagerInGame.ShowMenuButtonPanel();
        }

        public void DeclineDrawByRequestPopup()
        {
            text.text = drawDeclinedText2;
            EffectsPlayer.Instance.PlayNotifySound();

            CloseThisAfterTime();
        }

        private void CloseThisAfterTime()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(CloseThisPanelRoutine());
            }

            return;

            IEnumerator CloseThisPanelRoutine()
            {
                yield return new WaitForSeconds(3f);

                _thisMenuPanel.Hide();
                panelManagerInGame.ShowMenuButtonPanel();
            }
        }
    }
}
