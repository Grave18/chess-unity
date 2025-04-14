using Ui.MainMenu;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingsExitPanel : MonoBehaviour
{
    [Header("Ui")]
    [FormerlySerializedAs("settingsButton")]
    [SerializeField] private Button menuButton;

    [SerializeField] private MenuPanel menuPanel;

    private void OnEnable()
    {
        menuButton.onClick.AddListener(SettingsPopupShow);
    }

    private void OnDisable()
    {
        menuButton.onClick.RemoveListener(SettingsPopupShow);
    }

    private void SettingsPopupShow()
    {
        menuPanel.Show();
    }
}