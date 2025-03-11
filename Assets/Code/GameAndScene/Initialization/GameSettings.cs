using UnityEngine;

namespace GameAndScene.Initialization
{
    [System.Serializable]
    public class GameSettings
    {
        public const string Key = "GameSettings";

        [field: SerializeField]
        public PlayerSettings Player1Settings { get; set; }

        [field: SerializeField]
        public PlayerSettings Player2Settings { get; set; }

        [field: Space]
        [field: SerializeField]
        public string Fen { get; set; } = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    }
}