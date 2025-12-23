using Chess3D.Runtime.LoadScreen;
using Chess3D.Runtime.Utilities.Logging;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Chess3D.Runtime.Utilities
{
    [Preserve]
    public sealed class SceneManager
    {
        private const string LogTag = "SCENE";

        public async UniTask LoadScene(int toLoadIndex)
        {
            int currentSceneIndex = UnitySceneManager.GetActiveScene().buildIndex;

            bool isNeedLoadingScene = currentSceneIndex == RuntimeConstants.Scenes.Menu && toLoadIndex == RuntimeConstants.Scenes.Core
                                   || currentSceneIndex == RuntimeConstants.Scenes.Core && toLoadIndex == RuntimeConstants.Scenes.Menu;

            if (isNeedLoadingScene)
            {
                Log.Default.D(LogTag, "LoadingScreen is loading.");
                await UnitySceneManager.LoadSceneAsync(RuntimeConstants.Scenes.LoadScreen, LoadSceneMode.Additive);
                await LoadingSceneFader.Instance.Fade();

                Log.Default.D(LogTag, $"{SceneUtility.GetScenePathByBuildIndex(currentSceneIndex)} is unloading.");
                await UnitySceneManager.UnloadSceneAsync(currentSceneIndex);
                Log.Default.D(LogTag, $"{SceneUtility.GetScenePathByBuildIndex(toLoadIndex)} is loading.");
                await UnitySceneManager.LoadSceneAsync(toLoadIndex, LoadSceneMode.Additive);

                Log.Default.D(LogTag, "LoadingScreen is unloading.");
                await LoadingSceneFader.Instance.UnFade();
                await UnitySceneManager.UnloadSceneAsync(RuntimeConstants.Scenes.LoadScreen);

                return;
            }

            await UniTask.NextFrame();

            Log.Default.D(LogTag, $"{SceneUtility.GetScenePathByBuildIndex(toLoadIndex)} is loading.");
            UnitySceneManager.LoadScene(toLoadIndex, LoadSceneMode.Single);
        }

        public void SetActiveScene(int sceneIndex)
        {
            UnitySceneManager.SetActiveScene(UnitySceneManager.GetSceneByBuildIndex(sceneIndex));
        }
    }
}