using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UtilsProject.GameSetup;

using static PlayTests.TestUtils;

namespace PlayTests
{
    [TestFixture]
    public class StartGameTest
    {
        [UnityTest]
        [Timeout(2_147_483_647)]
        public IEnumerator LocalhostOnline_WithComputers_MustEnd()
        {
            yield return SceneManager.LoadSceneAsync("MainMenuScene");
            GameSettingsContainer.SetCurrentFen("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            Settings.GameSettingsContainer.IsOnlineComputerVsComputer = true;
            yield return LocalhostSetupLoader.Load().ToCoroutine();

            yield return new WaitUntil(() => Game.IsEndGame);

            Assert.IsTrue(Game.IsEndGame, "Game must be ended");

            CloneCommandSender.Send("ExitPlaymode");
        }
    }
}