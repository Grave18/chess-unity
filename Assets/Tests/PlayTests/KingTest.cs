using System.Collections;
using ChessBoard;
using ChessBoard.Pieces;
using Logic;
using Logic.Players;
using NUnit.Framework;
using Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace PlayTests
{
    [TestFixture]
    public class KingTest
    {
        [UnityTest]
        public IEnumerator Move_WhenTwoSquares_IsNotMovedToSquare()
        {
            yield return TestUtils.InitGameWithPreset("4k3/8/8/8/8/2P5/8/4K3 w - - 0 1");

            var game = Object.FindObjectOfType<Game>();
            var board = Object.FindObjectOfType<Board>();
            var currentPlayer = Object.FindObjectOfType<Competitors>().CurrentPlayer as PlayerOffline;

            Piece pieceGiven = board.GetSquare("e1").GetPiece();

            yield return TestUtils.Move("e1e3");

            Piece pieceExpected = board.GetSquare("e3").GetPiece();
            Assert.AreNotEqual(pieceExpected, pieceGiven);
        }

        [UnityTest]
        public IEnumerator Move_WhenMoveOnAttackedSquare_CanNotMoveToSquare()
        {
            yield return TestUtils.InitGameWithPreset("R3k3/8/8/8/8/5p2/8/4K3 w - - 0 1");

            var game = Object.FindObjectOfType<Game>();
            var board = Object.FindObjectOfType<Board>();
            var currentPlayer = Object.FindObjectOfType<Competitors>().CurrentPlayer as PlayerOffline;

            Piece pieceGiven = board.GetSquare("e1").GetPiece();

            yield return TestUtils.Move("e1e2");

            Piece pieceExpected = board.GetSquare("e3").GetPiece();
            Assert.AreNotEqual(pieceExpected, pieceGiven);
        }

        [UnityTest]
        public IEnumerator Check_WhenUnderAttack_StateIsCheck()
        {
            yield return TestUtils.InitGameWithPreset("4k3/8/8/8/8/5r2/8/4K3 b - - 0 1");

            var currentPlayer = Object.FindObjectOfType<Competitors>().CurrentPlayer as PlayerOffline;
            var game = Object.FindObjectOfType<Game>();

            Assert.AreEqual(CheckType.None, game.CheckType);

            yield return TestUtils.Move("f3e3");

            Assert.AreEqual(CheckType.Check, game.CheckType);
        }

        [UnityTest]
        public IEnumerator Check_WhenAttackGone_StateIsNone()
        {
            yield return TestUtils.InitGameWithPreset("4k3/8/8/8/8/4r3/8/4K3 b - - 0 1");

            var currentPlayer = Object.FindObjectOfType<Competitors>().CurrentPlayer as PlayerOffline;
            var game = Object.FindObjectOfType<Game>();

            Assert.AreEqual(CheckType.Check, game.CheckType);

            yield return TestUtils.Move("e3f3");

            Assert.AreEqual(CheckType.None, game.CheckType);
        }

        [UnityTest]
        public IEnumerator CheckMate_WhenUnderAttackAndCanNotMove_StateIsCheckMate()
        {
            yield return TestUtils.InitGameWithPreset("6k1/8/8/8/8/4r3/r7/1PKP4 b - - 0 1");

            var currentPlayer = Object.FindObjectOfType<Competitors>().CurrentPlayer as PlayerOffline;
            var game = Object.FindObjectOfType<Game>();

            yield return TestUtils.Move("e3c3");

            Assert.AreEqual(CheckType.CheckMate, game.CheckType);
        }

        [UnityTest]
        public IEnumerator Stalemate_WhenKingPinned_StateIsStalemate()
        {
            yield return TestUtils.InitGameWithPreset("4k3/8/8/8/6q1/8/8/7K b - - 0 1");

            var game = Object.FindObjectOfType<Game>();
            var currentPlayer = Object.FindObjectOfType<Competitors>().CurrentPlayer as PlayerOffline;

            yield return TestUtils.Move("g4g3");

            Assert.AreEqual(game.CheckType, CheckType.Draw);
        }

        [UnityTest]
        public IEnumerator Castling_WhenCanCastleAndCastling_MoveToSquare()
        {
            yield return TestUtils.InitGameWithPreset("4k3/8/8/8/8/8/8/4K2R w K - 0 1");

            var currentPlayer = Object.FindObjectOfType<Competitors>().CurrentPlayer as PlayerOffline;
            var game = Object.FindObjectOfType<Game>();

            Piece kingGiven = game.Board.GetSquare("e1").GetPiece();
            Piece rookGiven = game.Board.GetSquare("h1").GetPiece();

            yield return TestUtils.Move("e1g1");

            Piece kingExpected = game.Board.GetSquare("g1").GetPiece();
            Piece rookExpected = game.Board.GetSquare("f1").GetPiece();
            Assert.AreEqual(kingExpected, kingGiven);
            Assert.AreEqual(rookExpected, rookGiven);
        }

        [UnityTest]
        public IEnumerator Castling_WhenUnderAttack_CanNotMoveToSquare()
        {
            yield return TestUtils.InitGameWithPreset("4k3/8/8/8/8/4r3/8/4K2R w K - 0 1");

            var currentPlayer = Object.FindObjectOfType<Competitors>().CurrentPlayer as PlayerOffline;
            var game = Object.FindObjectOfType<Game>();

            Piece kingGiven = game.Board.GetSquare("e1").GetPiece();
            Piece rookGiven = game.Board.GetSquare("h1").GetPiece();

            yield return TestUtils.Move("e1g1");

            Piece kingExpected = game.Board.GetSquare("g1").GetPiece();
            Piece rookExpected = game.Board.GetSquare("f1").GetPiece();
            Assert.AreNotEqual(kingExpected, kingGiven);
            Assert.AreNotEqual(rookExpected, rookGiven);
        }

        [UnityTest]
        public IEnumerator Castling_WhenMoveLineUnderAttack_CanNotMoveToSquare()
        {
            yield return TestUtils.InitGameWithPreset("4k3/8/8/8/8/5r2/8/4K2R w K - 0 1");

            var currentPlayer = Object.FindObjectOfType<Competitors>().CurrentPlayer as PlayerOffline;
            var game = Object.FindObjectOfType<Game>();

            Piece kingGiven = game.Board.GetSquare("e1").GetPiece();
            Piece rookGiven = game.Board.GetSquare("h1").GetPiece();

            yield return TestUtils.Move("e1g1");

            Piece kingExpected = game.Board.GetSquare("g1").GetPiece();
            Piece rookExpected = game.Board.GetSquare("f1").GetPiece();
            Assert.AreNotEqual(kingExpected, kingGiven);
            Assert.AreNotEqual(rookExpected, rookGiven);
        }

        [UnitySetUp]
        public IEnumerator Setup()
        {
            yield return SceneManager.LoadSceneAsync("MainMenuScene");

            var gameSettingsContainerObj = Object.FindObjectOfType<GameSettingsContainer>();
            gameSettingsContainerObj.SetupGameOffline();

            yield return SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
            yield return SceneManager.UnloadSceneAsync("MainMenuScene");
        }
    }
}