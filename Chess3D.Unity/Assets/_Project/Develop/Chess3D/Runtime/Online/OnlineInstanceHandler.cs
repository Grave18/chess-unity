using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Logic.Players;
using Chess3D.Runtime.Utilities.Common.Singleton;
using PurrNet;
using UnityEngine;

namespace Chess3D.Runtime.Online
{
    public class OnlineInstanceHandler : SingletonBehaviour<OnlineInstanceHandler>
    {
        [Header("References")]
        [SerializeField] private PlayerOnline playerOnlineWhite;
        [SerializeField] private PlayerOnline playerOnlineBlack;
        [SerializeField] private ClockOnline clockOnline;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        public static bool IsOnline => Instance != null;
        public static bool IsOffline => !IsOnline;

        public static PlayerOnline PlayerWhite => Instance.playerOnlineWhite;
        public static PlayerOnline PlayerBlack => Instance.playerOnlineBlack;
        public static ClockOnline Clock => Instance.clockOnline;

        public static PlayerID ThisPlayerID { get; private set; }
        public static PlayerID OtherPlayerID { get; private set; }

        public static void Init(PlayerID thisPlayerId)
        {
            ThisPlayerID = thisPlayerId;
            if (thisPlayerId.id.value == 001)
            {
                OtherPlayerID = new PlayerID(002, false);
            }
            else if(thisPlayerId.id.value == 002)
            {
                OtherPlayerID = new PlayerID(001, false);
            }
            else
            {
                Debug.LogWarning($"{nameof(OnlineInstanceHandler)}: ThisPlayerID is not 001 or 002");
                OtherPlayerID = default;
            }
        }
    }
}