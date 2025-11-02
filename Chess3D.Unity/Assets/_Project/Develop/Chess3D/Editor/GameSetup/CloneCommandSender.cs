using UnityEngine;

namespace Chess3D.Editor.GameSetup
{
    public static class CloneCommandSender
    {
        public static void Send(string command)
        {
            PlayerPrefs.SetString("CommandToClone", command);
        }
    }
}