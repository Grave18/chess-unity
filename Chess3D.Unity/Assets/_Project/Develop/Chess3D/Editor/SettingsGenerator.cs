using System.IO;
using Chess3D.Runtime;
using Chess3D.Runtime.Core.Logic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using AudioSettings = Chess3D.Runtime.AudioSettings;
using PlayerSettings = Chess3D.Runtime.Bootstrap.Settings.PlayerSettings;

namespace Chess3D.Editor
{
    public static class SettingsGenerator
    {
        private static string InternalSettingsPath = Path.Combine(Application.dataPath, "_Project", "Resources", RuntimeConstants.Settings.FileName + ".json");

        [MenuItem("Chess3D/Settings/Generate Settings", priority = 1)]
        public static void Generate()
        {
            var settings = new SettingsContainer
            {
                AudioSettings = new AudioSettings
                {
                    MasterVolume = 1f,
                    MusicVolume = 1f,
                    EffectsVolume = 1f
                },
                GraphicsSettings = new GraphicsSettings
                {
                    Width = 0,
                    Height = 0,
                    FullScreenMode = 0,
                    Quality = 2,
                },
                GameSettings = new GameSettings
                {
                    PlayerColor = PieceColor.White,
                    Time = new Vector2Int(60,
                        0),
                    CurrentFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
                    DefaultFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
                    CustomFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
                    SavedFen = string.Empty,
                    IsAutoPromoteToQueen = false,
                    IsAutoRotateCamera = true,
                    IsRotateCameraOnStart = true,
                    FiftyMoveRuleCount = 50,
                    ThreefoldRepetitionCount = 3,
                    PlayerWhite = new PlayerSettings
                    {
                        Name = "Player 1",
                    },
                    PlayerBlack = new PlayerSettings
                    {
                        Name = "Player 2",
                    },
                    PiecesModelAddress = "Day4 Pieces",
                    BoardModelAddress = "Day4 Board",
                },
            };
            ;
            var settingsJson = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(InternalSettingsPath, settingsJson);
        }

        [MenuItem("Chess3D/Settings/Open External Settings", priority = 1)]
        public static void OpenExternalSettings()
        {
            string filePath = Path.Combine(Application.persistentDataPath, RuntimeConstants.Settings.FileName + ".json");
            InternalEditorUtility.OpenFileAtLineExternal(filePath, 1);
        }

        [MenuItem("Chess3D/Settings/Open Default Settings", priority = 1)]
        public static void OpenInternalSettings()
        {
            InternalEditorUtility.OpenFileAtLineExternal(InternalSettingsPath, 1);
        }
    }
}