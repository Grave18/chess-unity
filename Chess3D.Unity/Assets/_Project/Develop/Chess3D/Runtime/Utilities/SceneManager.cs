using Chess3D.Runtime.Bootstrap;
using Chess3D.Runtime.Utilities.Logging;
using System.Threading;
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

        private readonly LoadingScreen loadingScreen;

        public SceneManager(LoadingScreen loadingScreen)
        {
            this.loadingScreen = loadingScreen;
        }

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public async UniTask LoadScene(int toLoadIndex, CancellationToken cancellationToken = default)
        {
            int currentSceneIndex = UnitySceneManager.GetActiveScene().buildIndex;

            bool isFromMenuToGame = currentSceneIndex == RuntimeConstants.Scenes.Menu
                                    && toLoadIndex == RuntimeConstants.Scenes.Core
                                    || toLoadIndex == RuntimeConstants.Scenes.Online;

            bool isFromGameToMenu = currentSceneIndex == RuntimeConstants.Scenes.Core
                                    || currentSceneIndex == RuntimeConstants.Scenes.Online
                                    && toLoadIndex == RuntimeConstants.Scenes.Menu;

            if (isFromMenuToGame || isFromGameToMenu)
            {
                await UnitySceneManager.LoadSceneAsync(RuntimeConstants.Scenes.Blank);
                await loadingScreen.Fade();

                Log.Default.D(LogTag, $"{SceneUtility.GetScenePathByBuildIndex(toLoadIndex)} is loading.");
                await UnitySceneManager.LoadSceneAsync(toLoadIndex, LoadSceneMode.Single);

                await loadingScreen.Unfade();

                return;
            }

            await UniTask.NextFrame(cancellationToken);

            Log.Default.D(LogTag, $"{SceneUtility.GetScenePathByBuildIndex(toLoadIndex)} is loading.");
            UnitySceneManager.LoadScene(toLoadIndex, LoadSceneMode.Single);
        }
    }
}