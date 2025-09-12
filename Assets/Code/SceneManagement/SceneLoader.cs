using Cysharp.Threading.Tasks;
using Initialization;
using Network;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        public async UniTaskVoid LoadMainMenu()
        {
            await SceneManager.LoadSceneAsync(loadingScene, LoadSceneMode.Additive);
            await LoadingScene.Instance.Fade();

            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(gameScene);
            AsyncOperation loadOp =  SceneManager.LoadSceneAsync(mainMenuScene, LoadSceneMode.Additive);
            while (!loadOp!.isDone || !unloadOp!.isDone)
            {
                LoadingScene.Instance.LoadingProgress = loadOp.progress;
                await UniTask.NextFrame();
            }

            await LoadingScene.Instance.UnFade();
            await SceneManager.UnloadSceneAsync(loadingScene);
        }

        public async UniTaskVoid LoadGame()
        {
            await SceneManager.LoadSceneAsync(loadingScene, LoadSceneMode.Additive);
            await LoadingScene.Instance.Fade();

            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(mainMenuScene);
            AsyncOperation loadOp =  SceneManager.LoadSceneAsync(gameScene, LoadSceneMode.Additive);
            float timer = 1f;
            while (!loadOp!.isDone || !unloadOp!.isDone || timer > 0)
            {
                timer -= Time.deltaTime;
                LoadingScene.Instance.LoadingProgress = loadOp.progress;
                await UniTask.NextFrame();
            }

            await GameInitialization.Instance.Init();
            GameInitialization.Instance.StartGame();

            await LoadingScene.Instance.UnFade();
            await SceneManager.UnloadSceneAsync(loadingScene);
        }

        public async UniTaskVoid LoadGameOnline()
        {
            await SceneManager.LoadSceneAsync(loadingScene, LoadSceneMode.Additive);
            await LoadingScene.Instance.Fade();

            SceneManager.UnloadSceneAsync(mainMenuScene);

            if (!IsSceneLoaded(onlineScene))
            {
                await SceneManager.LoadSceneAsync(onlineScene, LoadSceneMode.Additive);
            }

            await OnlineInitialization.Instance.Init();
            await OnlineInitialization.Instance.LoadGame();

            await GameInitialization.Instance.Init();
            GameInitialization.Instance.StartGame();

            await LoadingScene.Instance.UnFade();
            await SceneManager.UnloadSceneAsync(loadingScene);
        }

        private static bool IsSceneLoaded(SceneReference sceneReference)
        {
            Scene scene = SceneManager.GetSceneByPath(sceneReference.ScenePath);
            return scene.isLoaded;
        }
    }
}