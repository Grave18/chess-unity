using System;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using PurrNet;
using PurrNet.Transports;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Online
{
    [Preserve]
    public class ConnectionTerminator : IDisposable
    {
        private readonly NetworkManager _networkManager;
        private readonly ConfigContainer _configContainer;
        private readonly SceneManager _sceneManager;

        public ConnectionTerminator(NetworkManager networkManager,
            ConfigContainer configContainer, SceneManager sceneManager)
        {
            _networkManager = networkManager;
            _configContainer = configContainer;
            _sceneManager = sceneManager;

            _networkManager.onClientConnectionState += OnClientConnectionState;
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
        }

        // Add delay to game scene to unload and load main menu
        private void ClientDisconnect()
        {
            _sceneManager.LoadScene(RuntimeConstants.Scenes.Menu).Forget();
        }
    }
}