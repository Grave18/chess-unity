using UnityEngine;

namespace UtilsProject.GameSetup
{
    public static class CloneCommandSender
    {
        public static void Send(string command)
        {
            PlayerPrefs.SetString("CommandToClone", command);
        }
    }
}