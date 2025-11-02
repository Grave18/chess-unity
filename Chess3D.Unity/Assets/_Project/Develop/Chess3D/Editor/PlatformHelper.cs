using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Chess3D.Editor
{
    public class PlatformHelper
    {
        public static int[] GetQualityLevels(BuildTarget buildTarget)
        {
            var activeBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(activeBuildTargetGroup);
            int[] levelsForPlatform = QualitySettings.GetActiveQualityLevelsForPlatform(namedBuildTarget.TargetName);

            return levelsForPlatform;
        }
    }
}