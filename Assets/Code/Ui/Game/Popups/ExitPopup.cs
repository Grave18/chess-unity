using GameAndScene;
using Notation;
using Ui.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Game.Popups
{
    public class ExitPopup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private FenFromBoard fenFromBoard;
        [SerializeField] private SceneLoader sceneLoader;

        [Header("UI")]
        [SerializeField] private Toggle saveBoardToggle;
        [SerializeField] private Button yesButton;


        public void OnEnable()
        {
            yesButton.onClick.AddListener(ExitToMainMenu);
        }

        public void OnDisable()
        {
            yesButton.onClick.RemoveListener(ExitToMainMenu);
        }

        private void ExitToMainMenu()
        {
            if (saveBoardToggle.isOn)
            {
                string fen = fenFromBoard.Get();
                gameSettingsContainer.SaveFen(fen);
            }

            sceneLoader.LoadMainMenu();
        }
    }
}
