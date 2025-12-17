using System;
using System.IO;
using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Chess3D.Runtime
{
    public interface ISettingsContainer : ILoadUnit
    {
        Settings S { get; }
        void Save();
    }

    [Preserve]
    public sealed class SettingsContainer : ISettingsContainer
    {
        public Settings S { get; private set; }

        private static readonly string FilePath = Path.Combine(Application.persistentDataPath, RuntimeConstants.Settings.FileName + ".json");

        public UniTask Load()
        {
            string settingsJson;
            if (File.Exists(FilePath))
            {
                settingsJson = File.ReadAllText(FilePath);
            }
            else
            {
                var asset = AssetService.R.Load<TextAsset>(RuntimeConstants.Settings.FileName);
                settingsJson = asset.text;
            }

            S = JsonUtility.FromJson<Settings>(settingsJson);

            return UniTask.CompletedTask;
        }

        public void Save()
        {
            string settingsJson = JsonUtility.ToJson(S, prettyPrint: true);
            File.WriteAllText(FilePath, settingsJson);
        }
    }

    [Serializable]
    public sealed class Settings
    {
        public GameSettings GameSettings;
        public GraphicsSettings GraphicsSettings;
        public AudioSettings AudioSettings;
    }

    [Serializable]
    public sealed class GameSettings
    {
        public PieceColor PlayerColor;

        public PlayerSettings PlayerWhite;
        public PlayerSettings PlayerBlack;

        // Game Settings
        public Vector2Int Time;
        public string CurrentFen;
        public string DefaultFen;
        public string SavedFen;
        public string CustomFen;

        // Qol Settings
        public bool IsAutoPromoteToQueen;
        public bool IsAutoRotateCamera;
        public bool IsRotateCameraOnStart;

        // Draw Rules
        public int FiftyMoveRuleCount;
        public int ThreefoldRepetitionCount;

        // Asset addresses
        public string PiecesModelAddress;
        public string BoardModelAddress;
    }

    [Serializable]
    public sealed class GraphicsSettings
    {
        public int Width;
        public int Height;
        public int FullScreenMode;
        public int Quality;
    }

    [Serializable]
    public sealed class AudioSettings
    {
        public float MasterVolume;
        public float MusicVolume;
        public float EffectsVolume;
    }
}