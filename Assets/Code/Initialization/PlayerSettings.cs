using Ai;

namespace Initialization
{
    [System.Serializable]
    public class PlayerSettings
    {
        public string Name;
        public PlayerType PlayerType = PlayerType.Human;
        public ComputerSkillLevel ComputerSkillLevel = ComputerSkillLevel.Medium;
        public int ComputerThinkTimeMs = 1000;
    }
}