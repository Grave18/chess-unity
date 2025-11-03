using System.Linq;
using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Menu.LobbyManagement;
using Cysharp.Threading.Tasks;
using ParrelSync;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Chess3D.Editor.GameSetup
{
    public static class LocalhostSetupLoader
    {
        [MenuItem("Tools/GameSetup/Play And Load Localhost ^#w")]
        public static void PlayAndLoadLocalhost()
        {
            LoadScene("MainMenuScene");
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.EnterPlaymode();
        }

        private static void LoadScene(string scene)
        {
            if (!ClonesManager.IsClone())
            {
                EditorSceneManager.SaveOpenScenes();
            }

            EditorSceneManager.OpenScene($"Assets/Assets/Scenes/{scene}.unity");
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                Load().Forget();
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            }
        }

        public static async UniTask Load()
        {
            await UniTask.WaitForSeconds(1f, ignoreTimeScale: true);

            if (ClonesManager.IsClone())
            {
                await StartCloneClient();
            }
            else
            {
                await OpenOrCreateAndOpenClone();
                await StartHost();
            }
        }

        private static async UniTask StartCloneClient()
        {
            var onlineSceneSwitcher = Object.FindObjectOfType<OnlineSceneSwitcher>();
            await onlineSceneSwitcher.SetupAndSwitchScene(PieceColor.Black, isHost: false, isLocal: true);
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

        private static async UniTask StartHost()
        {
            if (GameSettingsContainer.IsOnlineComputerVsComputer)
            {
                CloneCommandSender.Send("StartLocalhostComputers");
            }
            else
            {
                CloneCommandSender.Send("StartLocalhost");
            }

            var onlineSceneSwitcher = Object.FindObjectOfType<OnlineSceneSwitcher>();
            await onlineSceneSwitcher.SetupAndSwitchScene(PieceColor.White, isHost: true, isLocal: true);
        }
    }
}