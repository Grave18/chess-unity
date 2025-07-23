using UnityEngine;
using UnityEngine.SceneManagement;
using UtilsCommon.SceneTool;

namespace GameAndScene
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("Scenes")]
        [SerializeField] private SceneReference mainMenuScene;
        [SerializeField] private SceneReference gameScene;
        [SerializeField] private SceneReference onlineLobbyScene;
        [SerializeField] private SceneReference onlineLocalhostScene;
        [SerializeField] private SceneReference blankScene;

        public void LoadMainMenu()
        {
            LoadScene(mainMenuScene);
        }

        public void LoadGame()
        {
            LoadScene(gameScene);
        }

        public void LoadOnlineLobby()
        {
            LoadScene(onlineLobbyScene);
        }

        public void LoadOnlineLocalhost()
        {
            LoadScene(onlineLocalhostScene);
        }

        public void ReloadCurrentScene()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            LoadScene(currentSceneName);
        }

        private void LoadScene(string sceneName)
        {
            SceneManager.LoadSceneAsync(blankScene);
            SceneManager.LoadSceneAsync(sceneName);
        }
    }
}
