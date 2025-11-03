using UnityEngine.SceneManagement;

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
        }
    }
}