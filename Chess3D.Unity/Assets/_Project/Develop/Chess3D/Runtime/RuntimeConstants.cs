using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using ParrelSync;
#endif

namespace Chess3D.Runtime
{
    public static class RuntimeConstants
    {
        public static class Scenes
        {
            public static readonly int Bootstrap = SceneUtility.GetBuildIndexByScenePath("0.Bootstrap");
            public static readonly int Logo = SceneUtility.GetBuildIndexByScenePath("1.Logo");
            public static readonly int Menu = SceneUtility.GetBuildIndexByScenePath("2.Menu");
            public static readonly int Core = SceneUtility.GetBuildIndexByScenePath("3.Core");
            public static readonly int Online = SceneUtility.GetBuildIndexByScenePath("4.Online");
            public static readonly int LoadScreen = SceneUtility.GetBuildIndexByScenePath("5.LoadScreen");
            public static readonly int Blank = SceneUtility.GetBuildIndexByScenePath("6.Blank");
        }

        public static class Settings
        {
#if UNITY_EDITOR
            public static readonly string FileName = ClonesManager.IsClone() ? "SettingsClone" : "Settings";
#else
            public const string FileName = "Settings";
#endif
            public const string MasterVolumeKey = "MasterVolume";
            public const string MusicVolumeKey = "MusicVolume";
            public const string EffectsVolumeKey = "EffectsVolume";
        }

        public static class Configs
        {
            public const string FileName = "Configs";
        }
    }
}