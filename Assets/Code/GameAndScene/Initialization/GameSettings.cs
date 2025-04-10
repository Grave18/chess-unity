using UnityEngine;

namespace GameAndScene.Initialization
{
    [System.Serializable]
    public class GameSettings
    {
        public const string Key = "GameSettings";

        public PlayerSettings Player1Settings;
        public PlayerSettings Player2Settings;

        [Space]
        public Vector2 Time;
        public string Fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        [Space]
        public bool IsAutoPromoteToQueen;
    }
}