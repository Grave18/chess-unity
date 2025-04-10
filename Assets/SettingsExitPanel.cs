using UnityEngine;
using UnityEngine.UI;

public class SettingsExitPanel : MonoBehaviour
{
    [Header("Ui")]
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private ExitPopup exitPopup;
    [SerializeField] private SettingsPopup settingsPopup;

    private void OnEnable()
    {
        exitButton.onClick.AddListener(ExitPopupShow);
        settingsButton.onClick.AddListener(SettingsPopupShow);
    }

    private void OnDisable()
    {
        exitButton.onClick.RemoveListener(ExitPopupShow);
        settingsButton.onClick.RemoveListener(SettingsPopupShow);
    }

    private void ExitPopupShow()
    {
        exitPopup.Show();
    }

    private void SettingsPopupShow()
    {
        settingsPopup.Show();
    }
}