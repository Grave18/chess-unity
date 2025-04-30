#if UNITY_EDITOR

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

namespace Tools.BuildTool
{
    public static class PlayerBuilder
    {
        private static BuildSettings _buildSettings;
        private static string _locationPathName;
        private static BuildPlayerOptions _buildPlayerOptions;

        [MenuItem("Tools/Grave/Build Player &b")]
        public static void Build()
        {
            LoadSettingsIfNeeded();
            SetLocationPathName();
            SetBuildPlayerOptions();

            AddressableAssetSettings.BuildPlayerContent();
            BuildPipeline.BuildPlayer(_buildPlayerOptions);

            if (_buildSettings.AddSteamAppidFile)
            {
                using FileStream fs = File.Create(_buildSettings.BuildPath + "/steam_appid.txt");
                using StreamWriter sw = new(fs);

                sw.WriteLine(_buildSettings.SteamAppid);
            }
        }

        private static void LoadSettingsIfNeeded()
        {
            if (_buildSettings == null)
            {
                _buildSettings = AssetDatabase.LoadAssetAtPath<BuildSettings>("Assets/Settings/BuildSettings.asset");
            }
        }

        private static void SetLocationPathName()
        {
            string fileName = _buildSettings.BuildTarget switch
            {
                BuildTarget.StandaloneWindows or BuildTarget.StandaloneWindows64 => "chess.exe",
                BuildTarget.Android => "chess.app",
                BuildTarget.StandaloneOSX => "chess.app",
                BuildTarget.StandaloneLinux64 => "chess.x86_64",
                _ => "chess",
            };

            _locationPathName = _buildSettings.BuildPath + "/" + fileName;
        }

        private static void SetBuildPlayerOptions()
        {
            _buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes
                    .Where(s => s.enabled)
                    .Select(s => s.path)
                    .ToArray(),
                locationPathName = _locationPathName,
                targetGroup = _buildSettings.BuildTargetGroup,
                target = _buildSettings.BuildTarget,
                options = _buildSettings.BuildOptions,
            };
        }
    }
}

#endif