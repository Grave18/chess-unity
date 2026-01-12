using Chess3D.Runtime.Utilities.Logging;
using PurrNet;
using PurrNet.Transports;
using UnityEngine;

namespace Chess3D.Runtime.Online
{
    public class OnlineDebug : NetworkBehaviour
    {
#if !GRAVEBOX_PROD

        private const string LogTag = "CONNECTION";

        private void Awake()
        {
            if (InstanceHandler.NetworkManager == null)
            {
                return;
            }

            InstanceHandler.NetworkManager.onClientConnectionState += OnNetworkManagerOnClientConnectionState;
            InstanceHandler.NetworkManager.onServerConnectionState += OnNetworkManagerOnServerConnectionState;
            InstanceHandler.NetworkManager.onPlayerJoined += OnNetworkManagerOnPlayerJoined;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (InstanceHandler.NetworkManager == null)
            {
                return;
            }

            InstanceHandler.NetworkManager.onClientConnectionState -= OnNetworkManagerOnClientConnectionState;
            InstanceHandler.NetworkManager.onServerConnectionState -= OnNetworkManagerOnServerConnectionState;
            InstanceHandler.NetworkManager.onPlayerJoined -= OnNetworkManagerOnPlayerJoined;
        }

        private void OnNetworkManagerOnPlayerJoined(PlayerID player, bool reconnect, bool server) => Log.Default.D(LogTag, $"Player joined. Id: {player.id}, is reconnect: {reconnect}, as server: {server}");
        private void OnNetworkManagerOnServerConnectionState(ConnectionState state) => Log.Default.D(LogTag, $"Server state is {state}");
        private void OnNetworkManagerOnClientConnectionState(ConnectionState state) => Log.Default.D(LogTag, $"Client state is {state}");

        protected override void OnObserverAdded(PlayerID player)
        {
            Log.Default.D(LogTag, $"Observer added. Id: {player.id}");
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