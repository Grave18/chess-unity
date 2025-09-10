using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UtilsCommon;
using UtilsCommon.SceneTool;

namespace SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("Scenes")]
        [SerializeField] private SceneReference mainMenuScene;
        [SerializeField] private SceneReference loadingScene;
        [SerializeField] private SceneReference gameScene;
        [SerializeField] private SceneReference onlineScene;
        [SerializeField] private SceneReference blankScene;

        public void LoadMainMenu()
        {
            LoadScene(mainMenuScene, gameScene).RunCoroutine();
        }

        public void LoadGame()
        {
            LoadScene(gameScene, mainMenuScene).RunCoroutine();
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

            LoadScene(currentSceneName, currentSceneName).RunCoroutine();
        }

        private IEnumerator LoadScene(string sceneToLoad, string sceneToUnload)
        {
            yield return SceneManager.LoadSceneAsync(loadingScene, LoadSceneMode.Additive);
            yield return LoadingScene.Instance.Fade();

            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneToUnload);
            AsyncOperation loadOp =  SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
            while (!loadOp!.isDone || !unloadOp!.isDone)
            {
                LoadingScene.Instance.LoadingProgress = loadOp.progress;
                yield return null;
            }

            yield return LoadingScene.Instance.UnFade();
            yield return SceneManager.UnloadSceneAsync(loadingScene);
        }
    }
}