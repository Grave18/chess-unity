using System.Collections;
using Cysharp.Threading.Tasks;
using Logic;
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
        public IEnumerator LocalhostOnline_WithComputers_MustEnd()
        {
            yield return SceneManager.LoadSceneAsync("MainMenuScene");
            GameSettingsContainer.SetCurrentFen("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            GameSettingsContainer.SetupGameComputerVsComputerOnline(PieceColor.White);

            yield return LocalhostSetupLoader.Load(isLoadComputerPlayers: true).ToCoroutine();

            yield return new WaitUntil(() => TestUtils.Game.IsEndGame);

            Assert.IsTrue(TestUtils.Game.IsEndGame, "Game must be ended");

            CloneCommandSender.Send("ExitPlaymode");
        }
    }
}