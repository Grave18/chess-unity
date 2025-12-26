using PurrNet;
using UnityEngine;

namespace Chess3D.Runtime.Online
{
    public class OnlineDebug : MonoBehaviour
    {

    #if !GRAVEBOX_PROD
            private void OnGUI()
            {
                int playerCount = InstanceHandler.NetworkManager?.playerCount ?? 0;
                ulong playerID = InstanceHandler.NetworkManager?.localPlayer.id.value ?? 0;
                string text = $"Is Host: {InstanceHandler.NetworkManager?.isHost}, PlayerCount: {playerCount}, id: {playerID}";
                GUI.Label(new Rect(Screen.width*0.03f, Screen.height*0.96f, Screen.width, Screen.width*0.2f), text);
            }
    #endif

    }
}