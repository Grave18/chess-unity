using UnityEngine;

namespace UtilsProject.GameSetup
{
    public static class CommandSender
    {
        public static void Send(string command)
        {
            PlayerPrefs.SetString("CommandToClone", command);
        }
    }
}