#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace UtilsCommon.BuildTool
{
    public class PostBuildProcessor : IPostprocessBuildWithReport
    {
        public int callbackOrder { get; }

        public void OnPostprocessBuild(BuildReport report)
        {
            IncrementBuildVersion();
            Debug.Log($"Build finished. Version: {PlayerSettings.bundleVersion}");
        }

        private static void IncrementBuildVersion()
        {
            var version = PlayerSettings.bundleVersion;
            if (!int.TryParse(version, out int versionNumber))
            {
                versionNumber = 0;
            }

            versionNumber += 1;
            PlayerSettings.bundleVersion = versionNumber.ToString();
        }
    }
}

#endif