using Cysharp.Threading.Tasks;
using Initialization;
using PurrNet;
using Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class GameSceneLoader : MonoBehaviour
    {
        [Header("Scenes")]
        [PurrScene] [SerializeField] private string gameScene;

        public async UniTask Load()
        {
            if (GameSettingsContainer.IsHost)
            {
                await InstanceHandler.NetworkManager?.sceneModule.LoadSceneAsync(gameScene, LoadSceneMode.Additive);
            }
            else
            {
                await UniTask.WaitWhile(() => GameInitialization.Instance is null);
            }
        }
    }
}