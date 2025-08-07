using System.Collections;
using PurrNet;
using Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using UtilsCommon;
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
            if (!IsSceneLoaded(onlineLocalhostScene))
            {
                LoadGameFirstTime();
            }
            else
            {
                LoadGameSecondTime();
            }
        }

        private void LoadGameFirstTime()
        {
            SceneManager.LoadSceneAsync(onlineLocalhostScene, LoadSceneMode.Additive)!
                .completed += _ => SceneManager.UnloadSceneAsync(mainMenuScene)!
                .completed += _ => StartServerAndClient();
        }

        private void LoadGameSecondTime()
        {
            SceneManager.UnloadSceneAsync(mainMenuScene)!
                .completed += _ => StartServerAndClient();
        }

        private static void StartServerAndClient()
        {
            if (GameSettingsContainer.IsLocalhostServer)
            {
                InstanceHandler.NetworkManager?.StartServer();
            }

            CoroutineRunner.Run(StartClientRoutine());
            return;

            IEnumerator StartClientRoutine()
            {
                yield return new WaitForSeconds(1f);

                InstanceHandler.NetworkManager?.StartClient();
            }
        }

        private static bool IsSceneLoaded(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByPath(sceneName);
            return scene.isLoaded;
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
