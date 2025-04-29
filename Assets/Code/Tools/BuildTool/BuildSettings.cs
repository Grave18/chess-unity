using System.IO;
using EditorCools;
using UnityEditor;
using UnityEngine;

namespace Tools.BuildTool
{
    [CreateAssetMenu(menuName = "Build/Build Settings", fileName = "BuildSettings")]
    public class BuildSettings : ScriptableObject
    {
        [Header("Build Settings")]
        public string BuildPath;
        public BuildTargetGroup BuildTargetGroup = BuildTargetGroup.Standalone;
        public BuildTarget BuildTarget = BuildTarget.StandaloneWindows64;
        public BuildOptions BuildOptions;

        [Header("Development")]
        [SerializeField] private bool developmentBuild;
        [SerializeField] private bool allowDebugging;

        [Header("Profiler")]
        [SerializeField] private bool autoConnectProfiler;
        [SerializeField] private bool deepProfiling;
        [SerializeField] private bool waitForPlayerConnection;

        [Header("Build")]
        [SerializeField] private bool showBuildFolder;
        [SerializeField] private bool autoRunPlayer;
        [SerializeField] private bool clanBuild;

        private void Reset()
        {
            BuildPath = Path.Combine($"{Application.dataPath}", "..", "Build");
            BuildPath = Path.GetFullPath(BuildPath);
        }

        private void OnValidate()
        {
            BuildOptions = developmentBuild ? BuildOptions | BuildOptions.Development : BuildOptions & ~BuildOptions.Development;
            BuildOptions = allowDebugging ? BuildOptions | BuildOptions.AllowDebugging : BuildOptions & ~BuildOptions.AllowDebugging;
            BuildOptions = autoConnectProfiler ? BuildOptions | BuildOptions.ConnectWithProfiler : BuildOptions & ~BuildOptions.ConnectWithProfiler;
            BuildOptions = deepProfiling ? BuildOptions | BuildOptions.EnableDeepProfilingSupport : BuildOptions & ~BuildOptions.EnableDeepProfilingSupport;
            BuildOptions = waitForPlayerConnection ? BuildOptions | BuildOptions.WaitForPlayerConnection : BuildOptions & ~BuildOptions.WaitForPlayerConnection;
            BuildOptions = autoRunPlayer ? BuildOptions | BuildOptions.AutoRunPlayer : BuildOptions & ~BuildOptions.AutoRunPlayer;
            BuildOptions = showBuildFolder ? BuildOptions | BuildOptions.ShowBuiltPlayer : BuildOptions & ~BuildOptions.ShowBuiltPlayer;
            BuildOptions = clanBuild ? BuildOptions | BuildOptions.CleanBuildCache : BuildOptions & ~BuildOptions.CleanBuildCache;
        }

        [Button(space: 10)]
        private void ChooseBuildPath()
        {
            if (string.IsNullOrEmpty(BuildPath))
            {
                BuildPath = $"{Application.dataPath}";
            }

            string newBuildPath = EditorUtility.OpenFolderPanel("Choose Build Path", BuildPath, "Build");

            if (!string.IsNullOrEmpty(newBuildPath))
            {
                BuildPath = newBuildPath;
            }
        }
    }
}