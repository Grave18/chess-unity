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

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public async UniTask LoadScene(int toLoadIndex, bool isUseLoadingScreen = false)
        {
            int currentSceneIndex = UnitySceneManager.GetActiveScene().buildIndex;

            if (isUseLoadingScreen)
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
    }
}