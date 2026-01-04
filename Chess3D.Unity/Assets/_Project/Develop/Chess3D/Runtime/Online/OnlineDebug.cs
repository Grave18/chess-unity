using PurrNet;
using UnityEngine;

namespace Chess3D.Runtime.Online
{
    public class OnlineDebug : NetworkBehaviour
    {
#if !GRAVEBOX_PROD
        private void Awake()
        {
            InstanceHandler.NetworkManager.onClientConnectionState += state =>
            {
                Debug.Log($"Client state is {state}");
            };

            InstanceHandler.NetworkManager.onServerConnectionState += state =>
            {
                Debug.Log($"Server state is {state}");
            };

            InstanceHandler.NetworkManager.onPlayerJoined += (player, reconnect, server) =>
            {
                Debug.Log($"Player joined. Id: {player.id}, is reconnect: {reconnect}, as server: {server}");
            };
        }

        protected override void OnObserverAdded(PlayerID player)
        {
            Debug.Log($"Observer added. Id: {player.id}");
        }

        private void OnGUI()
        {
            if (InstanceHandler.NetworkManager == null)
            {
                return;
            }

            string authority = InstanceHandler.NetworkManager.isHost ? "Host" : "Client";
            int playerCount = InstanceHandler.NetworkManager.playerCount;
            ulong playerID = InstanceHandler.NetworkManager.localPlayer.id.value;
            string text = $"{authority}, PlayerCount: {playerCount}, id: {playerID:000}";

            GUI.Label(new Rect(Screen.width*0.03f, Screen.height*0.96f, Screen.width, Screen.width*0.2f), text);
        }
#endif

    }
}