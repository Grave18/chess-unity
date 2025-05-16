using System.Collections;
using Network.Localhost;
using PurrNet;
using TMPro;
using Ui.MainMenu;
using UnityEngine;

namespace Network
{
    public class SceneLoaderOnline : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] [PurrScene] private string gameScene;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        [Header("Debug Ui")]
        [SerializeField] private TMP_Text debugText;

        private int _connectionCount;

        private void Awake()
        {
            if (InstanceHandler.NetworkManager != null)
            {
                InstanceHandler.NetworkManager.onPlayerJoined += OnConnected;
            }
        }

        protected override void OnObserverAdded(PlayerID player)
        {
            base.OnObserverAdded(player);

            if (player.id == 002)
            {
                SetPreset(player, gameSettingsContainer.GetCurrentFen());
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

                if (_connectionCount == LocalhostPlayersCount.Get)
                {
                    StartCoroutine(LoadGame());
                }
            }
            else
            {
                debugText.text = $"Host: {InstanceHandler.NetworkManager.isHost}. Connections: {_connectionCount}";
            }
        }

        private IEnumerator LoadGame()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            InstanceHandler.NetworkManager.sceneModule.LoadSceneAsync(gameScene);
        }

        [TargetRpc]
        private void SetPreset(PlayerID target, string preset)
        {
            gameSettingsContainer.SetCurrentFen(preset);
        }
    }
}