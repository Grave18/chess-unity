using System.Collections;
using PurrNet;
using UnityEngine;
using UnityEngine.Assertions;

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
        if (PlayerPrefs.GetInt("IsServer") == 1)
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