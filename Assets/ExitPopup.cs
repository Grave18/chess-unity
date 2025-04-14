using GameAndScene;
using Notation;
using Ui.MainMenu;
using UnityEngine;
using UnityEngine.UI;

public class ExitPopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameSettingsContainer gameSettingsContainer;
    [SerializeField] private FenFromBoard fenFromBoard;
    [SerializeField] private SceneLoader sceneLoader;

    [Header("UI")]
    [SerializeField] private Toggle saveBoardToggle;
    [SerializeField] private Button yesButton;

    public bool IsOpened => gameObject.activeSelf;

    public void OnEnable()
    {
        yesButton.onClick.AddListener(ExitToMainMenu);
    }

    public void OnDisable()
    {
        yesButton.onClick.RemoveListener(ExitToMainMenu);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
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
