#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Chess3D.Runtime.Utilities.Common.BuildTool
{
    public class PostBuildProcessor : IPostprocessBuildWithReport, IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
            IncrementBuildVersion();
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            Debug.Log($"Build finished. Build version: {PlayerSettings.bundleVersion}");
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