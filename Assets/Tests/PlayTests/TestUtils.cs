using System.Collections;
using Cysharp.Threading.Tasks;
using Initialization;
using Logic;
using Logic.Players;
using Settings;
using UnityEngine;

namespace PlayTests
{
    public static class TestUtils
    {
        public static IEnumerator InitGameWithPreset(string fenPreset)
        {
            var gameSettingsContainer = Object.FindObjectOfType<GameSettingsContainer>();
            gameSettingsContainer.SetCurrentFen(fenPreset);

            var gameInitialization = Object.FindObjectOfType<GameInitialization>();
            yield return gameInitialization.Init().ToCoroutine();
            yield return gameInitialization.StartGame().ToCoroutine();
        }

        public static IEnumerator Move(string uci, PlayerOffline currentPlayer, Game game)
        {
            currentPlayer?.Move(uci);
            return new WaitUntil(() => game.GameStateMachine.StateName == "Idle");
        }

        public static IEnumerator Undo(Game game)
        {
            game.GameStateMachine.Undo();
            return new WaitUntil(() => game.GameStateMachine.StateName == "Idle");
        }
    }
}