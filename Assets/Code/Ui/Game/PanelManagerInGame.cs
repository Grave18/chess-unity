using ChessGame;
using Ui.MainMenu;
using UnityEngine;

namespace Ui.Game
{
    public class PanelManagerInGame : PanelManagerBase
    {
        [Header("References")]
        [SerializeField] private ChessGame.Logic.Game game;

        [Header("Popups")]
        [SerializeField] private MenuPanel exitPopup;
        [SerializeField] private MenuPanel endGamePopup;
        [SerializeField] private MenuPanel menuButtonPanel;

        private void OnEnable()
        {
            game.OnWarmup += ShowMenuButtonPanel;
            game.OnEnd += ShowEndGamePopup;
        }

        private void OnDisable()
        {
            game.OnWarmup -= ShowMenuButtonPanel;
            game.OnEnd -= ShowEndGamePopup;
        }

        /// Disable input if in Menu
        protected override void SetCurrentPanelHook()
        {
            GInput.IsEnabled = CurrentPanel == menuButtonPanel;
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