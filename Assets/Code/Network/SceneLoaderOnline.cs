using System.Collections;
using PurrNet;
using PurrNet.Transports;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class SceneLoaderOnline : NetworkBehaviour
    {
        [Header("References")]
        [PurrScene] [SerializeField] private string gameScene;
        [PurrScene] [SerializeField] private string onlineLocalhostScene;
        [PurrScene] [SerializeField] private string mainMenuScene;

        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        [Header("Debug Ui")]
        [SerializeField] private TMP_Text debugText;

        private int _playerCount;

        private void Awake()
        {
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.onPlayerJoined += OnPlayerJoined;
                InstanceHandler.NetworkManager.onClientConnectionState += OnClientConnectionState;
            }
        }

        protected override void OnDestroy()
        {
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.onPlayerJoined -= OnPlayerJoined;
                InstanceHandler.NetworkManager.onClientConnectionState -= OnClientConnectionState;
            }
        }

        private void Update()
        {
            LogOnScreen(_playerCount);
        }

        public static void DisconnectFromServer()
        {
            InstanceHandler.NetworkManager?.StopClient();
        }

        private void OnPlayerJoined(PlayerID player, bool isReconnect, bool asServer)
        {
            AcquirePlayerCount();

            if (!asServer)
            {
                return;
            }

            if (_playerCount == 2)
            {
                if (!IsSceneLoaded(gameScene))
                {
                    LoadGame();
                }
            }
        }

        private void AcquirePlayerCount()
        {
            _playerCount = InstanceHandler.NetworkManager?.playerCount ?? 0;
        }

        private static bool IsSceneLoaded(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            return scene.isLoaded;
        }

        private void LoadGame()
        {
            StartCoroutine(LoadGameRoutine());
            return;

            IEnumerator LoadGameRoutine()
            {
                yield return new WaitForSecondsRealtime(0.5f);

                if (InstanceHandler.NetworkManager != null)
                {
                    InstanceHandler.NetworkManager.sceneModule.LoadSceneAsync(gameScene, LoadSceneMode.Additive);
                }
            }
        }

        private void LogOnScreen(int playerCount)
        {
            debugText.text = $"Is Host: {InstanceHandler.NetworkManager?.isHost}, PlayerCount: {playerCount}";
        }

        private void OnClientConnectionState(ConnectionState state)
        {
            if (state == ConnectionState.Disconnected)
            {
                if (isServer)
                {
                    OnHostDisconnected();
                }
                else
                {
                    OnClientDisconnected();
                }
            }
        }

        private void OnHostDisconnected()
        {
            SceneManager.UnloadSceneAsync(gameScene)!
                .completed += _ => SceneManager.LoadSceneAsync(mainMenuScene, LoadSceneMode.Additive)!
                .completed += _ => InstanceHandler.NetworkManager?.StopServer();
        }

        private void OnClientDisconnected()
        {
            SceneManager.LoadSceneAsync(mainMenuScene, LoadSceneMode.Additive);
        }
    }
}