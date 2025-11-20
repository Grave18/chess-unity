using System.IO;
using Chess3D.Runtime;
using Chess3D.Runtime.Core.Logic;
using UnityEditor;
using UnityEngine;
using PlayerSettings = Chess3D.Runtime.Bootstrap.Settings.PlayerSettings;

namespace Chess3D.Editor
{
    public static class SettingsGenerator
    {
        [MenuItem("Tools/Grave/Generators/Generate Settings")]
        public static void Generate()
        {
            var settings = new Settings
            {
                AudioSettings = new Runtime.AudioSettings
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

            var path = Path.Combine(Application.dataPath, "_Project", "Resources", RuntimeConstants.Settings.FileName + ".json");
            var settingsJson = JsonUtility.ToJson(settings, prettyPrint: true);
            File.WriteAllText(path, settingsJson);
        }
    }
}