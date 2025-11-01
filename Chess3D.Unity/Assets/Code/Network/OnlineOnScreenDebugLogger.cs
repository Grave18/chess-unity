using PurrNet;
using TMPro;
using UnityEngine;

namespace Network
{
    public class OnlineOnScreenDebugLogger : MonoBehaviour
    {
        [Header("Debug Ui")]
        [SerializeField] private TMP_Text debugText;

        private static int PlayerCount => InstanceHandler.NetworkManager?.playerCount ?? 0;
        private static ulong PlayerID => InstanceHandler.NetworkManager?.localPlayer.id.value ?? 0;

        private void Update()
        {
            LogOnScreen();
        }

        private void LogOnScreen()
        {
            debugText.text = $"Is Host: {InstanceHandler.NetworkManager?.isHost}, PlayerCount: {PlayerCount}, id: {PlayerID}";
        }
    }
}