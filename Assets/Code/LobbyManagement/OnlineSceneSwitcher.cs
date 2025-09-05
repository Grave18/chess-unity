using System.Collections;
using Logic;
using SceneManagement;
using Settings;
using UnityEngine;
using UtilsCommon;

namespace LobbyManagement
{
    public class OnlineSceneSwitcher : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        private void OnEnable()
        {
            lobbyManager.OnAllReady.AddListener(StartOnlineFromLobby);
        }

        private void OnDisable()
        {
            lobbyManager.OnAllReady.RemoveListener(StartOnlineFromLobby);
        }

        private void StartOnlineFromLobby()
        {
            bool isLobbyOwner = lobbyManager.CurrentLobby is { IsValid: true, IsOwner: true };
            PieceColor playerColor = isLobbyOwner ? PieceColor.White : PieceColor.Black;

            lobbyManager.SetLobbyStarted();
            SetupAndSwitchScene(playerColor, isLobbyOwner).RunCoroutine();
        }

        public IEnumerator SetupAndSwitchScene(PieceColor playerColor, bool isHost)
        {
            if (!isHost)
            {
                yield return new WaitForSecondsRealtime(1f);
            }

            gameSettingsContainer.SetupGameOnline(playerColor);
            GameSettingsContainer.IsHost = isHost;

            sceneLoader.LoadOnline();
        }
    }
}