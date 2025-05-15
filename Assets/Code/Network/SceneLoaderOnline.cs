using Network.Localhost;
using PurrNet;
using TMPro;
using UnityEngine;

namespace Network
{
    public class SceneLoaderOnline : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] [PurrScene] private string gameScene;

        [Header("Debug Ui")]
        [SerializeField] private TMP_Text debugText;

        private readonly SyncVar<int> _connectionCount = new();

        private void Awake()
        {
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.onPlayerJoined += OnConnected;
            }
        }

        private void OnDestroy()
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
                _connectionCount.value += 1;

                if (_connectionCount == LocalhostPlayersCount.Get)
                {
                    LoadGame();
                }
            }
            else
            {
                debugText.text = $"Host: {InstanceHandler.NetworkManager.isHost}. Connections: {_connectionCount}";
            }
        }

        private void LoadGame()
        {
            InstanceHandler.NetworkManager.sceneModule.LoadSceneAsync(gameScene);
        }
    }
}
