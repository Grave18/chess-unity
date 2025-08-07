using Logic;
using Logic.Players;
using PurrNet;
using Settings;
using UnityEngine;
using UtilsCommon.Singleton;

namespace Network
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

        public static PlayerID ThisPlayerID => GetPlayerID(isThisPlayer: true);
        public static PlayerID OtherPlayerID => GetPlayerID(isThisPlayer: false);
        public static PlayerID WhitePlayerID => new(001, false);
        public static PlayerID BlackPlayerID => new(002, false);

        private static PlayerID GetPlayerID(bool isThisPlayer)
        {
            NetworkManager nm = InstanceHandler.NetworkManager;
            if (nm == null)
            {
                Debug.LogError($"{nameof(OnlineInstanceHandler)}: NetworkManager is null");
                return default;
            }

            if (nm.players.Count < 2)
            {
                Debug.LogError($"{nameof(OnlineInstanceHandler)}: NetworkManager has less than 2 players");
                return default;
            }

            if (GameSettingsContainer.IsLocalhostServer)
            {
                int index = isThisPlayer ? 0 : 1;
                return nm.players[index];
            }
            else
            {
                int index = isThisPlayer ? 1 : 0;
                return nm.players[index];
            }
        }
    }
}