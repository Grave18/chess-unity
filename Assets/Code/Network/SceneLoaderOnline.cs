using System.Collections;
using Network.Localhost;
using PurrNet;
using PurrNet.Modules;
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

        private int _connectionCount;

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

        private void OnPlayerJoined(PlayerID player, bool isReconnect, bool asServer)
        {
            if (asServer)
            {
                _connectionCount += 1;
                if (_connectionCount == LocalhostPlayersCount.Get)
                {
                    StartCoroutine(LoadGame());
                }
            }
            else
            {
                debugText.text = $"Host: {InstanceHandler.NetworkManager?.isHost}";
            }
        }

        private IEnumerator LoadGame()
        {
            yield return new WaitForSecondsRealtime(0.5f);

            ScenesModule sceneModule = InstanceHandler.NetworkManager?.sceneModule;
            sceneModule?.LoadSceneAsync(gameScene, LoadSceneMode.Additive);
        }

        public static void DisconnectFromServer()
        {
            InstanceHandler.NetworkManager?.StopClient();
        }
    }
}