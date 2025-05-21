using GameAndScene;
using Network;
using PurrNet;
using TMPro;
using Ui.MainMenu;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Ui.Game.Popups
{
    public class EndGamePopup : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private ChessGame.Logic.Game game;
        [SerializeField] private SceneLoader sceneLoader;

        [Header("UI")]
        [SerializeField] private PanelManagerInGame panelManagerInGame;
        [SerializeField] private MenuPanel rematchPopup;
        [SerializeField] private TMP_Text text;

        [Header("Buttons")]
        [SerializeField] private Button rematchButton;
        [SerializeField] private Button homeButton;

        [Header("Text")]
        [SerializeField] private string endgameText = "Game ended";
        [SerializeField] private string rematchRequestedText = "Rematch requested";
        [SerializeField] private string rematchDeclinedText = "Rematch declined";
        [SerializeField] private string wantToRematchText = "Want to rematch?";

        private MenuPanel thisMenuPanel;

        private void Awake()
        {
            thisMenuPanel = GetComponentInParent<MenuPanel>();
            Assert.IsNotNull(thisMenuPanel);
        }

        private void OnEnable()
        {
            rematchButton.onClick.AddListener(Rematch);
            homeButton.onClick.AddListener(ExitGame);

            RestoreState();
        }

        private void RestoreState()
        {
            text.text = endgameText;
            rematchButton.interactable = true;
        }

        private void OnDisable()
        {
            rematchButton.onClick.RemoveListener(Rematch);
            homeButton.onClick.RemoveListener(ExitGame);
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
            thisMenuPanel.Hide();
            game.RestartGame();
        }

        private void RematchOnline()
        {
            rematchButton.interactable = false;
            text.text = rematchRequestedText;

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

            rematchPopup.Show();
            EffectsPlayer.Instance.PlayNotifySound();
        }

        public void DeclineRematch()
        {
            text.text = rematchDeclinedText;
            rematchButton.interactable = true;
        }

        private void ExitGame()
        {
            panelManagerInGame.ShowExitPopup();
        }
    }
}
