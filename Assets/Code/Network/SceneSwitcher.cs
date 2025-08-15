using LobbyManagement;
using PurrLobby;
using PurrNet;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class SceneSwitcher : MonoBehaviour
    {
        [SerializeField] private LobbyManager lobbyManager;
        [PurrScene, SerializeField] private string nextScene;

        private void OnEnable()
        {
            lobbyManager.OnAllReady.AddListener(SwitchScene);
        }

        private void OnDisable()
        {
            lobbyManager.OnAllReady.RemoveListener(SwitchScene);
        }

        private void SwitchScene()
        {
            lobbyManager.SetLobbyStarted();
            SceneManager.LoadSceneAsync(nextScene);
        }
    }
}