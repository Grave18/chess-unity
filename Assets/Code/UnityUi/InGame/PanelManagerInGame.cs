using InputManagement;
using UnityEngine;
using UnityUi.Menu;

namespace UnityUi.InGame
{
    public class PanelManagerInGame : PanelManagerBase
    {
        [Header("References")]
        [SerializeField] private Logic.Game game;

        [Header("UI")]
        [SerializeField] private GameObject inputBlockingPanel;

        [Header("Popups")]
        [SerializeField] private MenuPanel exitPopup;
        [SerializeField] private MenuPanel endGamePopup;
        [SerializeField] private MenuPanel menuButtonPanel;

        private bool IsInMenu => CurrentPanel != menuButtonPanel;

        private void OnEnable()
        {
            game.OnWarmup += ShowMenuButtonPanel;
        }

        private void OnDisable()
        {
            game.OnWarmup -= ShowMenuButtonPanel;
        }

        /// Disable input if in Menu and enable input blocking
        protected override void SetCurrentPanelHook()
        {
            InputController.IsEnabled = !IsInMenu;
            inputBlockingPanel.SetActive(IsInMenu);
        }

        public void ShowMenuButtonPanel()
        {
            menuButtonPanel.Show();
        }

        public void ShowEndGamePopup()
        {
            endGamePopup.Show();
        }

        public void ShowExitPopup()
        {
            exitPopup.Show();
        }
    }
}