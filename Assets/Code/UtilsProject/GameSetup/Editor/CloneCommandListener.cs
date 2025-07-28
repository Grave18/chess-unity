using ParrelSync;
using UnityEditor;
using UnityEngine;

namespace UtilsProject.GameSetup
{
    [InitializeOnLoad]
    public class CloneCommandListener
    {
        static CloneCommandListener()
        {
            if (ClonesManager.IsClone())
            {
                EditorApplication.update += ListenToCommand;
            }
        }

        private static void ListenToCommand()
        {
            string command = PlayerPrefs.GetString("CommandToClone", string.Empty);

            switch (command)
            {
                case "StartLocalhost":
                    ExecuteLoadLocalhostCommand();
                    break;
            }

            PlayerPrefs.SetString("CommandToClone", string.Empty);
        }

        private static void ExecuteLoadLocalhostCommand()
        {
            LocalhostSetupLoader.LoadLocalhost();
        }
    }
}