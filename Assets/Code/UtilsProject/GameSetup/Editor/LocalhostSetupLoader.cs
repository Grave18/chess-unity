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
        [MenuItem("Tools/GameSetup/Load Localhost ^#w")]
        public static void LoadLocalhost()
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
                await StartClient(isLoadComputerPlayers);
            }
            else
            {
                await StartHost(isLoadComputerPlayers);
            }
        }

        private static async UniTask StartClient(bool isLoadComputerPlayers)
        {
            var onlineSceneSwitcher = Object.FindObjectOfType<OnlineSceneSwitcher>();
            await onlineSceneSwitcher.SetupAndSwitchScene(PieceColor.Black, isHost: false, isLocal: true, isLoadComputerPlayers);
        }

        private static async UniTask StartHost(bool isLoadComputerPlayers)
        {
            CloneCommandSender.Send("StartLocalhost");

            var onlineSceneSwitcher = Object.FindObjectOfType<OnlineSceneSwitcher>();
            await onlineSceneSwitcher.SetupAndSwitchScene(PieceColor.White, isHost: true, isLocal: true, isLoadComputerPlayers);
        }
    }
}