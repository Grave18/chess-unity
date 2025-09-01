using LobbyManagement;
using SceneManagement;
using UnityEngine;

namespace Network
{
    public class LobbySceneSwitcher : MonoBehaviour
    {
        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private SceneLoader sceneLoader;

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
            sceneLoader.LoadOnlineLobby();
        }
    }
}