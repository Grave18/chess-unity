using Ai;
using UnityEngine;

namespace GameAndScene.Initialization
{
    [System.Serializable]
    public class PlayerSettings
    {
        [field: SerializeField]
        public string Name { get; set; } = "Player";

        [field: SerializeField]
        public PlayerType PlayerType { get; set; } = PlayerType.Offline;

        [field: SerializeField]
        public ComputerSkillLevel ComputerSkillLevel { get; set; } = ComputerSkillLevel.Medium;

        [field: SerializeField]
        public int ComputerThinkTimeMs { get; set; } = 1000;
    }
}