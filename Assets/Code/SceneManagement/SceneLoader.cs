using UnityEngine;
using UnityEngine.SceneManagement;
using UtilsCommon.SceneTool;

namespace SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("Scenes")]
        [SerializeField] private SceneReference mainMenuScene;
        [SerializeField] private SceneReference gameScene;
        [SerializeField] private SceneReference onlineScene;
        [SerializeField] private SceneReference blankScene;

        public void LoadMainMenu()
        {
            LoadScene(mainMenuScene);
        }

        public void LoadGame()
        {
            LoadScene(gameScene);
        }

        public void LoadOnline()
        {
            if (!IsSceneLoaded(onlineScene))
            {
                LoadOnlineFirstTime();
            }
            else
            {
                LoadOnlineSecondTime();
            }
        }

        private void LoadOnlineFirstTime()
        {
            SceneManager.LoadSceneAsync(onlineScene, LoadSceneMode.Additive)!
                .completed += _ => SceneManager.UnloadSceneAsync(mainMenuScene);
        }

        private void LoadOnlineSecondTime()
        {
            SceneManager.UnloadSceneAsync(mainMenuScene);
        }

        private static bool IsSceneLoaded(SceneReference sceneReference)
        {
            Scene scene = SceneManager.GetSceneByPath(sceneReference.ScenePath);
            return scene.isLoaded;
        }

        public void ReloadCurrentScene()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            LoadScene(currentSceneName);
        }

        private AsyncOperation LoadScene(string sceneName)
        {
            AsyncOperation asyncOperation = null;

            SceneManager.LoadSceneAsync(blankScene)!
                .completed += _ => SceneManager.LoadSceneAsync(sceneName)!
                .completed += op => asyncOperation = op;

            return asyncOperation;
        }
    }
}