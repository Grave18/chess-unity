using System.Collections;
using PurrNet;
using Settings;
using UnityEngine;
using UnityEngine.Assertions;

namespace Network.Localhost
{
    [RequireComponent(typeof(NetworkManager))]
    public class ConnectionStarterLocalhost : MonoBehaviour
    {
        private NetworkManager _networkManager;

        private void Awake()
        {
            _networkManager = GetComponent<NetworkManager>();
            Assert.IsNotNull(_networkManager);
        }

        private void Start()
        {
            if (GameSettingsContainer.IsLocalhostServer)
            {
                _networkManager.StartServer();
            }

            StartCoroutine(StartClient());
        }

        private IEnumerator StartClient()
        {
            yield return new WaitForSeconds(1f);

            _networkManager.StartClient();
        }
    }
}