using Chess3D.Runtime.LoadScreen;
using Chess3D.Runtime.Online;
using Chess3D.Runtime.Utilities.Common.SceneTool;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: Delete this
namespace Chess3D.Runtime
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("Scenes")]
        [SerializeField] private SceneReference mainMenuScene;
        [SerializeField] private SceneReference loadingScene;
        [SerializeField] private SceneReference gameScene;
        [SerializeField] private SceneReference onlineScene;
        [SerializeField] private SceneReference blankScene;

        public async UniTask LoadMainMenu()
        {
            await SceneManager.LoadSceneAsync(loadingScene, LoadSceneMode.Additive);
            await LoadingSceneFader.Instance.Fade();

            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(gameScene);
            AsyncOperation loadOp =  SceneManager.LoadSceneAsync(mainMenuScene, LoadSceneMode.Additive);
            while (!loadOp!.isDone || !unloadOp!.isDone)
            {
                LoadingSceneFader.Instance.LoadingProgress = loadOp.progress;
                await UniTask.NextFrame();
            }

            await LoadingSceneFader.Instance.UnFade();
            await SceneManager.UnloadSceneAsync(loadingScene);
        }

        public async UniTask LoadGame()
        {
            await SceneManager.LoadSceneAsync(loadingScene, LoadSceneMode.Additive);
            await LoadingSceneFader.Instance.Fade();

            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(mainMenuScene);
            AsyncOperation loadOp =  SceneManager.LoadSceneAsync(gameScene, LoadSceneMode.Additive);
            float timer = 2f;
            while (!loadOp!.isDone || !unloadOp!.isDone || timer > 0)
            {
                timer -= Time.deltaTime;
                LoadingSceneFader.Instance.LoadingProgress = loadOp.progress;
                await UniTask.NextFrame();
            }

            // await GameInitialization.Instance.Init();

            await LoadingSceneFader.Instance.UnFade();
            await SceneManager.UnloadSceneAsync(loadingScene);

            // await GameInitialization.Instance.StartGame();
        }

        public async UniTask LoadGameOnline()
        {
            await SceneManager.LoadSceneAsync(loadingScene, LoadSceneMode.Additive);
            await LoadingSceneFader.Instance.Fade();

            SceneManager.UnloadSceneAsync(mainMenuScene);

            if (!IsSceneLoaded(onlineScene))
            {
                await SceneManager.LoadSceneAsync(onlineScene, LoadSceneMode.Additive);
            }

            await OnlineInitialization.Instance.Init();
            await OnlineInitialization.Instance.LoadGame();

            // await GameInitialization.Instance.Init();

            await LoadingSceneFader.Instance.UnFade();
            await SceneManager.UnloadSceneAsync(loadingScene);

            // await GameInitialization.Instance.StartGame();
        }

        private static bool IsSceneLoaded(SceneReference sceneReference)
        {
            Scene scene = SceneManager.GetSceneByPath(sceneReference.ScenePath);
            return scene.isLoaded;
        }
    }
}