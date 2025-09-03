using System;
using System.Collections;
using PurrNet;
using PurrNet.Transports;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UtilsCommon;

namespace Network
{
    public class SceneLoaderOnline : NetworkBehaviour
    {
        [Header("Scenes")]
        [PurrScene] [SerializeField] private string gameScene;
        [FormerlySerializedAs("onlineLocalhostScene")] [PurrScene] [SerializeField] private string onlineScene;
        [PurrScene] [SerializeField] private string mainMenuScene;

        [Header("References")]
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private PlayerJoinHandler playerJoinHandler;

        [Header("Debug Ui")]
        [SerializeField] private TMP_Text debugText;

        private int PlayerCount => InstanceHandler.NetworkManager?.playerCount ?? 0;

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
            LogOnScreen();
        }

        public static void DisconnectFromServer()
        {
            InstanceHandler.NetworkManager?.StopClient();
        }

        private void OnPlayerJoined(PlayerID player, bool isReconnect, bool asServer)
        {
            if (!asServer)
            {
                return;
            }

            if (PlayerCount == 2)
            {
                if (!IsSceneLoaded(gameScene))
                {
                    LoadGame().RunCoroutine();
                }
            }
        }

        private static bool IsSceneLoaded(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            return scene.isLoaded;
        }

        private IEnumerator LoadGame()
        {
            yield return new WaitUntil(IsPlayersExchangeData);

            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.sceneModule.LoadSceneAsync(gameScene, LoadSceneMode.Additive);
            }
        }

        private bool IsPlayersExchangeData()
        {
            return playerJoinHandler.IsAllPlayersExchangeData;
        }

        private void LogOnScreen()
        {
            debugText.text = $"Is Host: {InstanceHandler.NetworkManager?.isHost}, PlayerCount: {PlayerCount}";
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
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.sceneModule.UnloadSceneAsync(gameScene)!
                    .completed += _ => SceneManager.LoadSceneAsync(mainMenuScene, LoadSceneMode.Additive)!
                    .completed += _ => InstanceHandler.NetworkManager?.StopServer();
            }
            else
            {
                SceneManager.UnloadSceneAsync(gameScene)!
                    .completed += _ => SceneManager.LoadSceneAsync(mainMenuScene, LoadSceneMode.Additive)!
                    .completed += _ => InstanceHandler.NetworkManager?.StopServer();
            }
        }

        private void OnClientDisconnected()
        {
            CoroutineRunner.Run(ClientDisconnectedRoutine());
            return;

            // Add delay to game scene to unload and load main menu
            IEnumerator ClientDisconnectedRoutine()
            {
                yield return new WaitUntil(() => !IsSceneLoaded(gameScene));
                SceneManager.LoadSceneAsync(mainMenuScene, LoadSceneMode.Additive);
            }
        }
    }
}