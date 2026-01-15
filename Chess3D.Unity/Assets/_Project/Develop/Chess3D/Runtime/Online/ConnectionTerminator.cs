using System;
using Chess3D.Runtime.Utilities;
using Chess3D.Runtime.Utilities.Logging;
using Cysharp.Threading.Tasks;
using PurrNet;
using PurrNet.Transports;
using UnityEngine.Scripting;
using VContainer.Unity;

namespace Chess3D.Runtime.Online
{
    [Preserve]
    public class ConnectionTerminator : IDisposable, IInitializable
    {
        private readonly NetworkManager _networkManager;
        private readonly ConfigContainer _configContainer;
        private readonly SceneManager _sceneManager;

        private const string LogTag = "CONNECTION";

        public ConnectionTerminator(NetworkManager networkManager,
            ConfigContainer configContainer, SceneManager sceneManager)
        {
            _networkManager = networkManager;
            _configContainer = configContainer;
            _sceneManager = sceneManager;

            _networkManager.onClientConnectionState += OnClientConnectionState;
        }

        public void Initialize()
        {
            // For create instantly in build time
        }

        public void Dispose()
        {
            _networkManager.onClientConnectionState -= OnClientConnectionState;
        }

        private void OnClientConnectionState(ConnectionState state)
        {
            if (state == ConnectionState.Disconnected)
            {
                if (_configContainer.OnlineConfig.AuthorityType == AuthorityType.Host)
                {
                    HostDisconnect();
                }
                else
                {
                    ClientDisconnect();
                }
            }
        }

        private void HostDisconnect()
        {
            _networkManager?.StopServer();

            _sceneManager.LoadScene(RuntimeConstants.Scenes.Menu).Forget();

            Log.Default.D(LogTag, "Host disconnected");
        }

        // Add delay to game scene to unload and load main menu
        private void ClientDisconnect()
        {
            _sceneManager.LoadScene(RuntimeConstants.Scenes.Menu).Forget();

            Log.Default.D(LogTag, $"Client disconnected {_networkManager?.localPlayer.id.value}");
        }
    }
}