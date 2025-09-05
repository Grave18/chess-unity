using PurrNet;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class GameSceneLoader : MonoBehaviour
    {
        [Header("Scenes")]
        [PurrScene] [SerializeField] private string gameScene;

        public void Load()
        {
            if (!IsSceneLoaded(gameScene))
            {
                LoadGame();
            }
        }

        private static bool IsSceneLoaded(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            return scene.isLoaded;
        }

        private void LoadGame()
        {
            InstanceHandler.NetworkManager?.sceneModule.LoadSceneAsync(gameScene, LoadSceneMode.Additive);
        }
    }
}