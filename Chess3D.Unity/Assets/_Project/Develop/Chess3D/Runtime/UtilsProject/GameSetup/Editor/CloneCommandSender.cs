using UnityEngine;

namespace Chess3D.Runtime.UtilsProject.GameSetup.Editor
{
    public static class CloneCommandSender
    {
        public static void Send(string command)
        {
            PlayerPrefs.SetString("CommandToClone", command);
        }
    }
}