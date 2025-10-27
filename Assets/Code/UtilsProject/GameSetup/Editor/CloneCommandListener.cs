using ParrelSync;
using Settings;
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
                    LocalhostSetupLoader.PlayAndLoadLocalhost();
                    break;
                case "StartLocalhostComputers":
                    LocalhostSetupLoader.PlayAndLoadLocalhost();
                    GameSettingsContainer.IsOnlineComputerVsComputer = true;
                    break;
                case "ExitPlaymode":
                    EditorApplication.ExitPlaymode();
                    break;
            }

            PlayerPrefs.SetString("CommandToClone", string.Empty);
        }
    }
}