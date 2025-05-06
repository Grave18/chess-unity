using Ai;
using Logic;

namespace GameAndScene.Initialization
{
    [System.Serializable]
    public class PlayerSettings
    {
        public string Name = "Player";
        public PlayerType PlayerType = PlayerType.Offline;
        public ComputerSkillLevel ComputerSkillLevel = ComputerSkillLevel.Medium;
        public int ComputerThinkTimeMs = 1000;
    }
}