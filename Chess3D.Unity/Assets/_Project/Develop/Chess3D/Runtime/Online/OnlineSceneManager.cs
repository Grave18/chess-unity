using Chess3D.Runtime.LoadScreen;
using Cysharp.Threading.Tasks;
using PurrNet;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Online
{
    [Preserve]
    public class OnlineSceneManager
    {
        private readonly ConfigContainer _configContainer;

        public OnlineSceneManager(ConfigContainer configContainer)
        {
            _configContainer = configContainer;
        }

        public async UniTask LoadCore()
        {
            if (_configContainer.OnlineConfig.AuthorityType == AuthorityType.Host)
            {
                await InstanceHandler.NetworkManager?.sceneModule.LoadSceneAsync(RuntimeConstants.Scenes.Core, LoadSceneMode.Additive);
            }
            else
            {
                await UniTask.WaitUntil(IsGameSceneLoaded);
            }
        }

        public async UniTask LoadMenu()
        {
            await SceneManager.LoadSceneAsync(RuntimeConstants.Scenes.LoadScreen, LoadSceneMode.Additive);
            await LoadingSceneFader.Instance.Fade();

            await UnloadGame();
            await SceneManager.LoadSceneAsync(RuntimeConstants.Scenes.Menu, LoadSceneMode.Additive);

            await LoadingSceneFader.Instance.UnFade();
            await SceneManager.UnloadSceneAsync(RuntimeConstants.Scenes.LoadScreen);
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
            if (_configContainer.OnlineConfig.AuthorityType == AuthorityType.Host)
            {
                InstanceHandler.NetworkManager.sceneModule.UnloadSceneAsync(RuntimeConstants.Scenes.Core);
            }
            else
            {
                await UniTask.WaitWhile(IsGameSceneLoaded);
            }
        }

        private bool IsGameSceneLoaded()
        {
            return SceneManager.GetSceneByBuildIndex(RuntimeConstants.Scenes.Core).isLoaded;
        }

        private async UniTask UnloadOffline()
        {
            await SceneManager.UnloadSceneAsync(RuntimeConstants.Scenes.Core);
        }
    }
}