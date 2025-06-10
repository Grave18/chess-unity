using System.Collections;
using GameAndScene;
using PurrNet;
using UnityEngine;
using UnityEngine.Assertions;

namespace Network.Localhost
{
    [RequireComponent(typeof(NetworkManager))]
    public class ConnectionStarterLocalhost : MonoBehaviour
    {
        private NetworkManager networkManager;

        private void Awake()
        {
            networkManager = GetComponent<NetworkManager>();
            Assert.IsNotNull(networkManager);
        }

        private void Start()
        {
            if (GameSettingsContainer.IsLocalhostServer)
            {
                networkManager.StartServer();
            }

            StartCoroutine(StartClient());
        }

        private IEnumerator StartClient()
        {
            yield return new WaitForSeconds(1f);

            networkManager.StartClient();
        }
    }
}