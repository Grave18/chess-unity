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
        private static readonly string ConfigPath = Path.Combine(Application.dataPath, "_Project", "Resources", RuntimeConstants.Configs.FileName + ".json");

        [MenuItem("Chess3D/Configs/Generate Config", priority = 0)]
        public static void Generate()
        {
            var config = new ConfigContainer
            {
                LobbyConfig = new LobbyConfig
                {
                    UpdateIntervalSec = 3f,
                    PlayersToStart = 2,
                },
                ChessConfig = new ChessConfig
                {
                    WarmupTimeSec = 3f,
                    UndoMoveTimeSec = 0.1f,
                    RedoMoveTimeSec = 0.1f,
                    MoveTimeSec = 0.3f,
                }
            };

            string configJson = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(ConfigPath, configJson);
        }

        [MenuItem("Chess3D/Configs/Open Config", priority = 0)]
        public static void Open()
        {
            InternalEditorUtility.OpenFileAtLineExternal(ConfigPath, 1);
        }
    }
}