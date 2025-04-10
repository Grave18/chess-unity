using Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitPopup : MonoBehaviour
{
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private SceneReference sceneReference;

    public void OnEnable()
    {
        yesButton.onClick.AddListener(ExitToMainMenu);
        noButton.onClick.AddListener(Hide);
    }

    public void OnDisable()
    {
        yesButton.onClick.RemoveListener(ExitToMainMenu);
        noButton.onClick.RemoveListener(Hide);
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
        SceneManager.LoadSceneAsync(sceneReference);
    }
}
