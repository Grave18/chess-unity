using System.Collections;
using Chess3D.Runtime.Core.Logic;
using NUnit.Framework;
using UnityEngine.TestTools;
using static Chess3D.Tests.PlayTests.TestUtils;

namespace Chess3D.Tests.PlayTests
{
    [TestFixture]
    public class DrawRulesTest
    {
        [UnitySetUp]
        public IEnumerator Setup()
        {
            yield return TestSetup();
            // GameSettingsContainer.RestoreDefaultRules();
        }

        [UnityTest]
        public IEnumerator FiftyMoveRule_WhenNormal_MustBeDraw()
        {
            // GameSettingsContainer.FiftyMoveRuleCount = 3;
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
            // GameSettingsContainer.FiftyMoveRuleCount = 3;
            yield return InitGameWithPreset("rnbqkbn1/8/8/8/8/8/r7/1NPPKPNR w - - 0 1");

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
            // GameSettingsContainer.FiftyMoveRuleCount = 3;
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
            // GameSettingsContainer.FiftyMoveRuleCount = 3;
            yield return InitGameWithPreset("rnbqkbnr/8/2p5/8/8/4P3/8/RNBQKBNR w KQq - 0 1");

            yield return Move("h1h3");
            yield return Move("h8h5");
            yield return Move("f1d3");
            yield return Move("h5h8");
            yield return Move("d3f1");
            yield return Move("h8h1"); // capture

            Assert.AreNotEqual(CheckType.Draw, TestUtils.Game.CheckType);
        }

        [UnityTest]
        public IEnumerator ThreeFoldRule_WhenNormal_MustBeDraw()
        {
            yield return InitGameWithPreset("k3r3/8/8/8/8/8/8/K3R2R w - - 0 1");

            // 1st e1 e8
            yield return Move("e1e2");
            yield return Move("e8e7");
            // 2nd e1 e8
            yield return Move("e2e1");
            yield return Move("e7e8");
            yield return Move("e1e2");
            yield return Move("e8e7");
            // 3rd e1 e8
            yield return Move("e2e1");
            yield return Move("e7e8");

            Assert.AreEqual(CheckType.Draw, TestUtils.Game.CheckType);
        }

        [UnityTest]
        public IEnumerator ThreeFoldRule_WhenFirstPositionWithCastling_MustBeDraw()
        {
            yield return InitGameWithPreset("q3k3/8/8/8/8/8/8/4K2R w K - 0 1");

            // 1st e1 e8
            yield return Move("e1e2");
            yield return Move("e8e7");
            // 2nd e1 e8
            yield return Move("e2e1");
            yield return Move("e7e8");
            yield return Move("e1e2");
            yield return Move("e8e7");
            // 3rd e1 e8
            yield return Move("e2e1");
            yield return Move("e7e8");
            Assert.AreEqual(CheckType.None, TestUtils.Game.CheckType, "In this position must be None. Because initial castling position");

            // Because first position is castling we must make one more move with white king
            yield return Move("e1e2");
            Assert.AreEqual(CheckType.Draw, TestUtils.Game.CheckType);
        }

        [UnityTest]
        public IEnumerator InsufficientMaterialRule_WhenInitialKK_MustBeDraw()
        {
            yield return InitGameWithPreset("4k3/8/8/8/8/8/8/4K3 w - - 0 1");

            Assert.AreEqual(CheckType.Draw, TestUtils.Game.CheckType);
        }

        [UnityTest]
        public IEnumerator InsufficientMaterialRule_WhenInitialKKB_MustBeDraw()
        {
            yield return InitGameWithPreset("6k1/8/8/5b2/8/8/8/7K b - - 0 1");

            Assert.AreEqual(CheckType.Draw, TestUtils.Game.CheckType);
        }

        [UnityTest]
        public IEnumerator InsufficientMaterialRule_WhenInitialKKN_MustBeDraw()
        {
            yield return InitGameWithPreset("6k1/8/8/8/3N4/8/8/7K b - - 0 1");

            Assert.AreEqual(CheckType.Draw, TestUtils.Game.CheckType);
        }

        [UnityTest]
        public IEnumerator InsufficientMaterialRule_WhenInitialKKNN_MustBeDraw()
        {
            yield return InitGameWithPreset("6k1/8/8/8/1n3n2/8/8/7K b - - 0 1");

            Assert.AreEqual(CheckType.Draw, TestUtils.Game.CheckType);
        }

        [UnityTest]
        public IEnumerator InsufficientMaterialRule_WhenKKAndCapturePawn_MustBeDraw()
        {
            yield return InitGameWithPreset("4k3/8/3P4/8/8/8/8/4K3 w - - 0 1");

            yield return Move("d6d7");
            yield return Move("e8d7");

            Assert.AreEqual(CheckType.Draw, TestUtils.Game.CheckType);
        }
    }
}