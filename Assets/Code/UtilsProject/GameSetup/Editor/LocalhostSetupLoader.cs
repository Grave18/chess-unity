using System.Linq;
using Cysharp.Threading.Tasks;
using LobbyManagement;
using Logic;
using ParrelSync;
using UnityEditor;
using UnityEngine;

namespace UtilsProject.GameSetup
{
    public static class LocalhostSetupLoader
    {
        [MenuItem("Tools/GameSetup/Play And Load Localhost ^#w")]
        public static void PlayAndLoadLocalhost()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.EnterPlaymode();
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                Load().Forget();
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            }
        }

        public static async UniTask Load(bool isLoadComputerPlayers = false)
        {
            await UniTask.WaitForSeconds(1f, ignoreTimeScale: true);

            if (ClonesManager.IsClone())
            {
                await StartCloneClient(isLoadComputerPlayers);
            }
            else
            {
                await OpenOrCreateAndOpenClone();
                await StartHost(isLoadComputerPlayers);
            }
        }

        private static async UniTask StartCloneClient(bool isLoadComputerPlayers)
        {
            var onlineSceneSwitcher = Object.FindObjectOfType<OnlineSceneSwitcher>();
            await onlineSceneSwitcher.SetupAndSwitchScene(PieceColor.Black, isHost: false, isLocal: true, isLoadComputerPlayers);
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

        private static async UniTask StartHost(bool isLoadComputerPlayers)
        {
            CloneCommandSender.Send("StartLocalhost");

            var onlineSceneSwitcher = Object.FindObjectOfType<OnlineSceneSwitcher>();
            await onlineSceneSwitcher.SetupAndSwitchScene(PieceColor.White, isHost: true, isLocal: true, isLoadComputerPlayers);
        }
    }
}