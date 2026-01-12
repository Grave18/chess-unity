using System.Linq;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Utilities;
using Chess3D.Runtime.Utilities.Logging;
using Cysharp.Threading.Tasks;
using ParrelSync;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Chess3D.Editor.GameSetup
{
    public static class LocalhostSetupLoader
    {
        private const string LogTag = "LOCALHOSTLOADER";

        [MenuItem("Chess3D/GameSetup/Play And Load Localhost ^#w")]
        public static void PlayAndLoadLocalhost()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            if (EditorApplication.isPlaying)
            {
                Log.Default.W(LogTag, "Exiting playmode first...");
                EditorApplication.ExitPlaymode();
                return;
            }

            EditorApplication.EnterPlaymode();
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                Load().Forget();
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            }
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                if (!ClonesManager.IsClone())
                {
                    SceneLoader.LoadBootstrap();
                }

                Log.Default.D(LogTag, "Entering play mode ...");
                EditorApplication.EnterPlaymode();
            }
        }

        public static async UniTask Load()
        {
            await UniTask.WaitForSeconds(1f, ignoreTimeScale: true);

            if (ClonesManager.IsClone())
            {
                StartCloneClient();
            }
            else
            {
                await OpenOrCreateAndOpenClone();
                StartHost();
            }
        }

        private static void StartCloneClient()
        {
            var onlineSceneSwitcher = ServiceLocator.Resolve<PlayViewModel>();
            onlineSceneSwitcher.StartLocalClient();
        }

        private static async UniTask OpenOrCreateAndOpenClone()
        {
            string clonePath = ClonesManager.GetCloneProjectsPath().FirstOrDefault();

            CreateCloneIfNotCreated(ref clonePath);
            OpenCloneIfNotOpened(clonePath);

            await UniTask.WaitUntil(() => ClonesManager.IsCloneProjectRunning(clonePath));
        }

        private static void CreateCloneIfNotCreated(ref string clonePath)
        {
            if (string.IsNullOrEmpty(clonePath))
            {
                Project cloneProject = ClonesManager.CreateCloneFromCurrent();
                clonePath = cloneProject.projectPath;
            }
        }

        private static void OpenCloneIfNotOpened(string clonePath)
        {
            // if cloned but not created fully?
            if (!ClonesManager.IsCloneProjectRunning(clonePath))
            {
                ClonesManager.OpenProject(clonePath);
            }
        }

        private static void StartHost()
        {
            // if (GameSettingsContainer.IsOnlineComputerVsComputer)
            // {
            //     CloneCommandSender.Send("StartLocalhostComputers");
            // }

            CloneCommandSender.Send("StartLocalhost");

            var onlineSceneSwitcher = ServiceLocator.Resolve<PlayViewModel>();
            onlineSceneSwitcher.StartLocalServer();
        }
    }
}