using System.Collections;
using ChessBoard;
using Cysharp.Threading.Tasks;
using Initialization;
using Logic;
using Logic.Players;
using Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayTests
{
    public static class TestUtils
    {
        public static Game Game => Object.FindObjectOfType<Game>();
        public static PlayerOffline PlayerOffline => Object.FindObjectOfType<Competitors>().CurrentPlayer as PlayerOffline;
        public static Board Board => Object.FindObjectOfType<Board>();

        public static IEnumerator TestSetup()
        {
            yield return SceneManager.LoadSceneAsync("MainMenuScene");

            var gameSettingsContainer = Object.FindObjectOfType<GameSettingsContainer>();
            gameSettingsContainer.SetupGameOffline();

            yield return SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
            yield return SceneManager.UnloadSceneAsync("MainMenuScene");
        }

        public static IEnumerator InitGameWithPreset(string fenPreset, bool isAutoRotateCamera = false, bool isRotateCameraOnStart = false)
        {
            var gameSettingsContainer = Object.FindObjectOfType<GameSettingsContainer>();
            gameSettingsContainer.SetCurrentFen(fenPreset);
            gameSettingsContainer.IsAutoRotateCamera = isAutoRotateCamera;
            gameSettingsContainer.IsRotateCameraOnStart = isRotateCameraOnStart;

            var gameInitialization = Object.FindObjectOfType<GameInitialization>();
            yield return gameInitialization.Init().ToCoroutine();
            yield return gameInitialization.StartGame().ToCoroutine();
        }

        public static IEnumerator Move(string uci)
        {
            PlayerOffline.Move(uci);
            return new WaitUntil(() => Game.GameStateMachine.StateName == "Idle");
        }

        public static IEnumerator Undo()
        {
            Game.GameStateMachine.Undo();
            return new WaitUntil(() => Game.GameStateMachine.StateName == "Idle");
        }
    }
}