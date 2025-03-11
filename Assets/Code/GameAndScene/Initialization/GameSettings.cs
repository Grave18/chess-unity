using UnityEngine;

namespace GameAndScene.Initialization
{
    [System.Serializable]
    public class GameSettings
    {
        public const string Key = "GameSettings";

        [field: Space]
        public string Player1Name {get; set;}
        public PlayerType PlayerWhite {get; set;}

        [field: Space]
        public string Player2Name {get; set;}
        public PlayerType PlayerBlack {get; set;}

        [field: Space]
        public string Fen {get; set;}
    }
}