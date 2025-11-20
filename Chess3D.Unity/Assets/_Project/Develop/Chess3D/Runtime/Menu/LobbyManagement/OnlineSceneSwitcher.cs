using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.Logic;
using Cysharp.Threading.Tasks;
using LobbyManagement;
using UnityEngine;

namespace Chess3D.Runtime.Menu.LobbyManagement
{
    public class OnlineSceneSwitcher : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private SceneLoader sceneLoader;
        // [SerializeField] private GameSettingsContainer gameSettingsContainer;

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

        public async UniTask SetupAndSwitchScene(PieceColor playerColor, bool isHost, bool isLocal = false)
        {
            if (!isHost)
            {
                await UniTask.WaitForSeconds(1f, ignoreTimeScale: true);
            }

            // if (GameSettingsContainer.IsOnlineComputerVsComputer)
            // {
                // gameSettingsContainer.SetupGameComputerVsComputerOnline(playerColor);
                // GameSettingsContainer.IsOnlineComputerVsComputer = false;
            // }
            // else
            // {
                // gameSettingsContainer.SetupGameHumanVsHumanOnline(playerColor);
            // }

            // GameSettingsContainer.IsHost = isHost;
            // GameSettingsContainer.IsLocal = isLocal;

            await sceneLoader.LoadGameOnline();
        }
    }
}