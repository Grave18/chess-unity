using Cysharp.Threading.Tasks;
using Logic;
using SceneManagement;
using Settings;
using UnityEngine;

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
            SetupAndSwitchScene(playerColor, isLobbyOwner).Forget();
        }

        public async UniTask SetupAndSwitchScene(PieceColor playerColor, bool isHost, bool isLocal = false,
            bool isLoadComputerPlayers = false)
        {
            if (!isHost)
            {
                await UniTask.WaitForSeconds(1f, ignoreTimeScale: true);
            }

            if (isLoadComputerPlayers)
            {
                gameSettingsContainer.SetupGameOnlineWithComputer(playerColor);
            }
            else
            {
                gameSettingsContainer.SetupGameOnline(playerColor);
            }

            GameSettingsContainer.IsHost = isHost;
            GameSettingsContainer.IsLocal = isLocal;

            await sceneLoader.LoadGameOnline();
        }
    }
}