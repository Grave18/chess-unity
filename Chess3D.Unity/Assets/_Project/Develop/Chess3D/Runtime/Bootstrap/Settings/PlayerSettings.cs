using System;
using Chess3D.Runtime.Core.Ai;

namespace Chess3D.Runtime.Bootstrap.Settings
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