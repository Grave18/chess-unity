using Chess3D.Runtime.Utilities.Logging;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Chess3D.Runtime.Utilities
{
    public class SceneManager
    {
        private const string LogTag = "SCENE";

        public async UniTask LoadScene(int toLoadIndex)
        {
            int currentSceneIndex = UnitySceneManager.GetActiveScene().buildIndex;
            bool isSkipEmpty = currentSceneIndex == RuntimeConstants.Scenes.Bootstrap || toLoadIndex == currentSceneIndex;

            if (isSkipEmpty)
            {
                Log.Default.D(LogTag, $"Empty scene skipped. {SceneUtility.GetScenePathByBuildIndex(toLoadIndex)} is loading.");
                UnitySceneManager.LoadScene(toLoadIndex);
                return;
            }

            bool needLoadEmpty = toLoadIndex == RuntimeConstants.Scenes.Menu || toLoadIndex == RuntimeConstants.Scenes.Core;

            if (needLoadEmpty)
            {
                Log.Default.D(LogTag, $"{SceneUtility.GetScenePathByBuildIndex(RuntimeConstants.Scenes.LoadScreen)} is loading.");
                UnitySceneManager.LoadScene(RuntimeConstants.Scenes.LoadScreen);
            }

            await UniTask.NextFrame();

            Log.Default.D(LogTag, $"{SceneUtility.GetScenePathByBuildIndex(toLoadIndex)} is loading.");
            UnitySceneManager.LoadScene(toLoadIndex);
        }

        public void SetActiveScene(int sceneIndex)
        {
            UnitySceneManager.SetActiveScene(UnitySceneManager.GetSceneByBuildIndex(sceneIndex));
        }
    }
}