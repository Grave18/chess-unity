using LobbyManagement;
using Logic;
using SceneManagement;
using Settings;
using UnityEngine;

namespace Network
{
    public class LobbySceneSwitcher : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

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
            bool isLobbyOwner = lobbyManager.CurrentLobby.IsOwner;
            PieceColor playerColor = isLobbyOwner ? PieceColor.White : PieceColor.Black;

            SetupOnlineLobbyGame(playerColor);

            lobbyManager.SetLobbyStarted();
            sceneLoader.LoadOnline();
        }

        private void SetupOnlineLobbyGame(PieceColor playerColor)
        {
            gameSettingsContainer.SetupGameOnline(playerColor);
            GameSettingsContainer.IsLocalhostServer = false;
        }
    }
}