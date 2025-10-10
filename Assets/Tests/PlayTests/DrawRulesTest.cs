using System.Collections;
using Logic;
using NUnit.Framework;
using UnityEngine.TestTools;
using static PlayTests.TestUtils;

namespace PlayTests
{
    [TestFixture]
    public class DrawRulesTest
    {
        [UnitySetUp]
        public IEnumerator Setup()
        {
            yield return TestSetup();
        }

        [UnityTest]
        public IEnumerator FiftyMoveRule_WhenNormal_MustBeDraw()
        {
            GameSettingsContainer.FiftyMoveRuleCount = 3;
            yield return InitGameWithPreset("rnbqkbnr/8/2p5/8/8/4P3/8/RNBQKBNR w KQq - 0 1");

            yield return Move("h1h3");
            yield return Move("h8h5");
            yield return Move("f1d3");
            yield return Move("h5h8");
            yield return Move("d3f1");
            yield return Move("h8h5");

            Assert.AreEqual(CheckType.Draw, TestUtils.Game.CheckType);
        }

        [UnityTest]
        public IEnumerator FiftyMoveRule_WhenLastIsCheckmate_MustBeCheckmate()
        {
            GameSettingsContainer.FiftyMoveRuleCount = 3;
            yield return InitGameWithPreset("rnbqkbn1/8/8/8/8/8/r7/1NPPKPNR w - - 0 1", isRotateCameraOnStart: true);

            yield return Move("h1h8");
            yield return Move("a8a5");
            yield return Move("a2a3");
            yield return Move("a3a2");
            yield return Move("g1h3");
            yield return Move("a5e5"); // checkmate

            Assert.AreEqual(CheckType.Checkmate, TestUtils.Game.CheckType);
        }

        [UnityTest]
        public IEnumerator FiftyMoveRule_WhenPawnMove_MustNotBeDraw()
        {
            GameSettingsContainer.FiftyMoveRuleCount = 3;
            yield return InitGameWithPreset("rnbqkbnr/8/2p5/8/8/4P3/8/RNBQKBNR w KQq - 0 1");

            yield return Move("h1h3");
            yield return Move("h8h5");
            yield return Move("f1d3");
            yield return Move("h5h8");
            yield return Move("d3f1");
            yield return Move("c6c5"); // pawn move

            Assert.AreNotEqual(CheckType.Draw, TestUtils.Game.CheckType);
        }

        [UnityTest]
        public IEnumerator FiftyMoveRule_WhenCapture_MustNotBeDraw()
        {
            GameSettingsContainer.FiftyMoveRuleCount = 3;
            yield return InitGameWithPreset("rnbqkbnr/8/2p5/8/8/4P3/8/RNBQKBNR w KQq - 0 1");

            yield return Move("h1h3");
            yield return Move("h8h5");
            yield return Move("f1d3");
            yield return Move("h5h8");
            yield return Move("d3f1");
            yield return Move("h8h1"); // capture

            Assert.AreNotEqual(CheckType.Draw, TestUtils.Game.CheckType);
        }

        // [UnityTest]
        // public IEnumerator ThreeFoldRule()
        // {
        //     yield return null;
        //     Assert.Pass();
        // }
        //
        // [UnityTest]
        // public IEnumerator InsufficientFiguresRule()
        // {
        //     yield return null;
        //     Assert.Pass();
        // }
    }
}