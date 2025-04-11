using Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameAndScene
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("Scenes")]
        [SerializeField] private SceneReference mainMenuScene;
        [SerializeField] private SceneReference gameScene;
        [SerializeField] private SceneReference blankScene;

        public void LoadMainMenu()
        {
            SceneManager.LoadSceneAsync(blankScene);
            SceneManager.LoadSceneAsync(mainMenuScene);
        }

        public void LoadGame()
        {
            SceneManager.LoadSceneAsync(blankScene);
            SceneManager.LoadSceneAsync(gameScene);
        }

        public void ReloadCurrentScene()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            SceneManager.LoadSceneAsync(blankScene);
            SceneManager.LoadSceneAsync(currentSceneName);
        }
    }
}
