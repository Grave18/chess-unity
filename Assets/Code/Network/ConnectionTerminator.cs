using System.Collections;
using PurrNet;
using PurrNet.Transports;
using Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using UtilsCommon;

namespace Network
{
    public class ConnectionTerminator : NetworkBehaviour
    {
        [Header("Scenes")]
        [PurrScene] [SerializeField] private string gameScene;
        [PurrScene] [SerializeField] private string onlineScene;
        [PurrScene] [SerializeField] private string mainMenuScene;

        [Header("References")]
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        private void Awake()
        {
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.onClientConnectionState += OnClientConnectionState;
            }
        }

        protected override void OnDestroy()
        {
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.onClientConnectionState -= OnClientConnectionState;
            }
        }

        public static void DisconnectFromServer()
        {
            InstanceHandler.NetworkManager?.StopClient();
        }

        private void OnClientConnectionState(ConnectionState state)
        {
            if (state == ConnectionState.Disconnected)
            {
                if (isServer)
                {
                    HostDisconnect();
                }
                else
                {
                    ClientDisconnect().RunCoroutine();
                }
            }
        }

        private void HostDisconnect()
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

        // Add delay to game scene to unload and load main menu
        private IEnumerator ClientDisconnect()
        {
            yield return new WaitUntil(() => !IsSceneLoaded(gameScene));
            SceneManager.LoadSceneAsync(mainMenuScene, LoadSceneMode.Additive);
        }

        private static bool IsSceneLoaded(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            return scene.isLoaded;
        }
    }
}