using Notation;
using SceneManagement;
using Settings;
using UnityEngine;
using UnityEngine.UI;
using UnityUi.Common.Buttons;

namespace UnityUi.InGame.Popups
{
    public class ExitPopup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private FenFromBoard fenFromBoard;
        [SerializeField] private SceneLoader sceneLoader;

        [Header("UI")]
        [SerializeField] private Toggle saveBoardToggle;
        [SerializeField] private ButtonBase yesButton;


        public void OnEnable()
        {
            yesButton.OnClick += ExitToMainMenu;
        }

        public void OnDisable()
        {
            yesButton.OnClick -= ExitToMainMenu;
        }

        private void ExitToMainMenu()
        {
            if (saveBoardToggle.isOn)
            {
                string fen = fenFromBoard.Get();
                gameSettingsContainer.SetSavedFen(fen);
            }

            sceneLoader.LoadMainMenu();
        }
    }
}
