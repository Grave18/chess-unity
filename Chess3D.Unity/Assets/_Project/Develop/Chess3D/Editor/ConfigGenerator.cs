using System.IO;
using Chess3D.Runtime;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Chess3D.Editor
{
    public static class ConfigGenerator
    {
        [MenuItem("Chess3D/Configs/Generate Config", priority = 0)]
        public static void Generate()
        {
            var config = new ConfigContainer
            {
                LobbyConfig = new LobbyConfig
                {
                    UpdateIntervalSec = 3f,
                    PlayersToStart = 2
                }
            };

            string configJson = JsonConvert.SerializeObject(config, Formatting.Indented);
            var configPath = Path.Combine(Application.dataPath, "_Project", "Resources", RuntimeConstants.Configs.FileName + ".json");
            File.WriteAllText(configPath, configJson);
        }

        [MenuItem("Chess3D/Configs/Open Config", priority = 0)]
        public static void Open()
        {
            var configPath = Path.Combine(Application.dataPath, "_Project", "Resources", RuntimeConstants.Configs.FileName + ".json");
            InternalEditorUtility.OpenFileAtLineExternal(configPath, 1);
        }
    }
}