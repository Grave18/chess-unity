using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.LoadScreen;
using Cysharp.Threading.Tasks;
using PurrNet;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chess3D.Runtime.Online
{
    public class OnlineSceneLoader : MonoBehaviour
    {
        [Header("Scenes")]
        [SerializeField] [PurrScene] private string gameScene;
        [SerializeField] [PurrScene] private string mainMenuScene;
        [SerializeField] [PurrScene] private string loadingScene;

        public async UniTask LoadGame()
        {
            // if (GameSettingsContainer.IsHost)
            // {
            //     await InstanceHandler.NetworkManager?.sceneModule.LoadSceneAsync(gameScene, LoadSceneMode.Additive);
            // }
            // else
            // {
            //     await UniTask.WaitUntil(IsGameSceneLoaded);
            // }
        }

        public async UniTask UnloadGameAndLoadMainMenu()
        {
            await SceneManager.LoadSceneAsync(loadingScene, LoadSceneMode.Additive);
            await LoadingSceneFader.Instance.Fade();

            await UnloadGame();
            await SceneManager.LoadSceneAsync(mainMenuScene, LoadSceneMode.Additive);

            await LoadingSceneFader.Instance.UnFade();
            await SceneManager.UnloadSceneAsync(loadingScene);
        }

        private async UniTask UnloadGame()
        {
            if (InstanceHandler.NetworkManager != null)
            {
                await UnloadOnline();
            }
            else
            {
                await UnloadOffline();
            }
        }

        private async UniTask UnloadOnline()
        {
            // if (GameSettingsContainer.IsHost)
            // {
            //     InstanceHandler.NetworkManager.sceneModule.UnloadSceneAsync(gameScene);
            // }
            // else
            // {
            //     await UniTask.WaitWhile(IsGameSceneLoaded);
            // }
        }

        private bool IsGameSceneLoaded()
        {
            return SceneManager.GetSceneByName(gameScene).isLoaded;
        }

        private async UniTask UnloadOffline()
        {
            await SceneManager.UnloadSceneAsync(gameScene);
        }
    }
}