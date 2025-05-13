using ChessGame.Logic;
using ChessGame.Logic.Players;
using UnityEngine;
using Utils;

namespace Network
{
    public class OnlineInstanceHandler : SingletonBehaviour<OnlineInstanceHandler>
    {
        [Header("References")]
        [SerializeField] private PlayerOnline playerOnlineWhite;
        [SerializeField] private PlayerOnline playerOnlineBlack;
        [SerializeField] private ClockOnline clockOnline;

        public static PlayerOnline PlayerWhite => Instance.playerOnlineWhite;
        public static PlayerOnline PlayerBlack => Instance.playerOnlineBlack;
        public static ClockOnline Clock => Instance.clockOnline;
    }
}