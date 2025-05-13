using PurrNet;
using UnityEngine;

namespace Network
{
    public class SceneLoaderOnline : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] [PurrScene] private string gameScene;

        [Header("Settings")]
        [SerializeField] private int connectionCount = 2;

        private int _connectionCount;

        private void Awake()
        {
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.onPlayerJoined += OnConnected;
            }
        }

        protected override void OnDestroy()
        {
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.onPlayerJoined -= OnConnected;
            }
        }

        private void OnConnected(PlayerID player, bool isReconnect, bool asServer)
        {
            if (asServer)
            {
                _connectionCount += 1;

                if (_connectionCount == connectionCount)
                {
                    LoadGame();
                }
            }
        }

        private void LoadGame()
        {
            InstanceHandler.NetworkManager.sceneModule.LoadSceneAsync(gameScene);
        }
    }
}
