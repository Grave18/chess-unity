using Notation;
using TMPro;
using Ui.MainMenu;
using UnityEngine;
using UnityEngine.UI;

public class FenPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameSettingsContainer gameSettingsContainer;

    [Header("Buttons")] [SerializeField] private Toggle defaultToggle;
    [SerializeField] private Toggle savedToggle;
    [SerializeField] private Toggle customToggle;

    [Header("Tabs")] [SerializeField] private GameObject defaultTab;
    [SerializeField] private GameObject savedTab;
    [SerializeField] private GameObject customTab;

    private TMP_InputField _customInputField;
    private TMP_Text _savedText;

    private void Awake()
    {
        _customInputField = customTab.GetComponentInChildren<TMP_InputField>();
        _savedText = savedTab.GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        defaultToggle.onValueChanged.AddListener(SetDefault);
        savedToggle.onValueChanged.AddListener(SetSaved);
        customToggle.onValueChanged.AddListener(SetCustom);

        _customInputField.onEndEdit.AddListener(OnEndEdit);

        SetupTabs();
    }

    private void SetupTabs()
    {
        defaultToggle.isOn = true;
        savedToggle.isOn = false;
        customToggle.isOn = false;

        SetDefault(true);
        SetSaved(false);
        SetCustom(false);

        _savedText.text = gameSettingsContainer.GetSavedFen();
        _customInputField.text = gameSettingsContainer.GetCurrentFen();
    }

    private void OnDisable()
    {
        defaultToggle.onValueChanged.RemoveListener(SetDefault);
        savedToggle.onValueChanged.RemoveListener(SetSaved);
        customToggle.onValueChanged.RemoveListener(SetCustom);

        _customInputField.onEndEdit.RemoveListener(OnEndEdit);
    }

    private void SetDefault(bool value)
    {
        defaultTab.SetActive(value);

        if (value)
        {
            gameSettingsContainer.SetDefaultFen();
        }
    }

    private void SetSaved(bool value)
    {
        savedTab.SetActive(value);

        if (value)
        {
            gameSettingsContainer.SetSavedFen();
        }
    }

    private void SetCustom(bool value)
    {
        customTab.SetActive(value);

        if (value)
        {
            OnEndEdit(_customInputField.text);
        }
    }

    private void OnEndEdit(string fen)
    {
        if (FenValidator.IsValid(fen, out string errorMessage))
        {
            gameSettingsContainer.SetFen(fen);
        }
        else
        {
            Debug.LogWarning(errorMessage);
        }
    }
}