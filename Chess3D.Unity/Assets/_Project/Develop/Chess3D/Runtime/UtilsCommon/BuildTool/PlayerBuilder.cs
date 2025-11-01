#if UNITY_EDITOR

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

namespace Chess3D.Runtime.UtilsCommon.BuildTool
{
    public static class PlayerBuilder
    {
        private static BuildSettings _buildSettings;
        private static string _locationPathName;
        private static BuildPlayerOptions _buildPlayerOptions;

        [MenuItem("Tools/Grave/Build Player &b")]
        public static void Build()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.ExitPlaymode();
                EditorApplication.playModeStateChanged += PlaymodeStateChanged;
            }
            else
            {
                BuildInternal();
            }
        }

        private static void PlaymodeStateChanged(PlayModeStateChange playMode)
        {
            if (playMode == PlayModeStateChange.EnteredEditMode)
            {
                BuildInternal();
                EditorApplication.playModeStateChanged -= PlaymodeStateChanged;
            }
        }

        private static void BuildInternal()
        {
            LoadSettingsIfNeeded();
            SetLocationPathName();
            SetBuildPlayerOptions();

            AddressableAssetSettings.BuildPlayerContent();
            BuildPipeline.BuildPlayer(_buildPlayerOptions);

            AddSteamAppId();
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
            string projectName = PlayerSettings.productName;
            string fileName = _buildSettings.BuildTarget switch
            {
                BuildTarget.StandaloneWindows or BuildTarget.StandaloneWindows64 => $"{projectName}.exe",
                BuildTarget.Android => $"{projectName}.app",
                BuildTarget.StandaloneOSX => $"{projectName}.app",
                BuildTarget.StandaloneLinux64 => $"{projectName}.x86_64",
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

        private static void AddSteamAppId()
        {
            if (_buildSettings.AddSteamAppidFile)
            {
                using FileStream fs = File.Create(_buildSettings.BuildPath + "/steam_appid.txt");
                using StreamWriter sw = new(fs);

                sw.WriteLine(_buildSettings.SteamAppid);
            }
        }
    }
}

#endif