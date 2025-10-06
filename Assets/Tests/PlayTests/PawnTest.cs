using System.Collections;
using ChessBoard;
using ChessBoard.Pieces;
using Cysharp.Threading.Tasks;
using Initialization;
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
    public class PawnTest
    {
        private const float MoveSec = 2f;
        private const float UndoSec = 1f;

        [UnityTest]
        public IEnumerator Move_WhenOneSquare_PieceIsMovedToToSquare()
        {
            yield return InitGameWithPreset("3k4/8/8/8/8/8/3P4/4K3 w - - 0 1");

            var board = Object.FindObjectOfType<Board>();
            Piece pieceGiven = board.GetSquare("d2").GetPiece();

            var currentPlayer = Object.FindObjectOfType<Competitors>().CurrentPlayer as PlayerOffline;
            currentPlayer?.Move("d2d3");

            yield return new WaitForSecondsRealtime(MoveSec);

            Piece pieceExpected = board.GetSquare("d3").GetPiece();

            Assert.AreEqual(pieceGiven, pieceExpected);
        }

        [UnityTest]
        public IEnumerator Move_WhenTwoSquares_IfFirstMove_PieceIsMovedToToSquare()
        {
            yield return InitGameWithPreset("3k4/8/8/8/8/8/3P4/4K3 w - - 0 1");

            var board = Object.FindObjectOfType<Board>();
            Piece pieceGiven = board.GetSquare("d2").GetPiece();

            var currentPlayer = Object.FindObjectOfType<Competitors>().CurrentPlayer as PlayerOffline;
            currentPlayer?.Move("d2d4");

            yield return new WaitForSecondsRealtime(MoveSec);

            Piece pieceExpected = board.GetSquare("d4").GetPiece();

            Assert.AreEqual(pieceGiven, pieceExpected);
        }

        [UnityTest]
        public IEnumerator Move_WhenTwoSquares_IfNotFirstMove_PieceIsNotMovedToSquare()
        {
            yield return InitGameWithPreset("4k3/8/8/8/8/3P4/8/4K3 w - - 0 1");

            var board = Object.FindObjectOfType<Board>();

            var currentPlayer = Object.FindObjectOfType<Competitors>().CurrentPlayer as PlayerOffline;
            currentPlayer?.Move("d3d5");

            yield return new WaitForSecondsRealtime(MoveSec);

            Piece nullPieceExpected = board.GetSquare("d5").GetPiece();

            Assert.Null(nullPieceExpected);
        }

        [UnityTest]
        public IEnumerator Move_WhenThreeSquares_PieceIsNotMovedToSquare()
        {
            yield return InitGameWithPreset("3k4/8/8/8/8/8/3P4/4K3 w - - 0 1");

            var board = Object.FindObjectOfType<Board>();
            Piece pieceGiven = board.GetSquare("d2").GetPiece();

            var currentPlayer = Object.FindObjectOfType<Competitors>().CurrentPlayer as PlayerOffline;
            currentPlayer?.Move("d2d5");

            yield return new WaitForSecondsRealtime(MoveSec);

            Piece pieceExpected = board.GetSquare("d5").GetPiece();

            Assert.AreNotEqual(pieceGiven, pieceExpected);
        }

        [UnityTest]
        public IEnumerator Capture_WhenCapturedPieceIsLeftOrRight_PieceIsCaptured()
        {
            yield return InitGameWithPreset("4k3/8/8/8/8/2ppp3/3P4/4K3 w - - 0 1");

            var board = Object.FindObjectOfType<Board>();
            var currentPlayer = Object.FindObjectOfType<Competitors>().CurrentPlayer as PlayerOffline;
            var game = Object.FindObjectOfType<Game>();

            Piece pieceGiven = board.GetSquare("d2").GetPiece();

            // Move Left
            currentPlayer?.Move("d2c3");
            yield return new WaitForSecondsRealtime(MoveSec);

            Piece pieceExpected = board.GetSquare("c3").GetPiece();
            Assert.AreEqual(pieceGiven, pieceExpected);

            // Undo
            game.GameStateMachine.Undo();
            yield return new WaitForSecondsRealtime(UndoSec);

            // Move Right
            currentPlayer?.Move("d2e3");
            yield return new WaitForSecondsRealtime(MoveSec);

            pieceExpected = board.GetSquare("e3").GetPiece();
            Assert.AreEqual(pieceGiven, pieceExpected);
        }

        [UnityTest]
        public IEnumerator Capture_WhenCapturedPieceIsFront_PieceIsNotCaptured()
        {
            yield return InitGameWithPreset("4k3/8/8/8/8/2ppp3/3P4/4K3 w - - 0 1");

            var board = Object.FindObjectOfType<Board>();
            var currentPlayer = Object.FindObjectOfType<Competitors>().CurrentPlayer as PlayerOffline;

            Piece pieceGiven = board.GetSquare("d2").GetPiece();

            currentPlayer?.Move("d2d3");
            yield return new WaitForSecondsRealtime(MoveSec);

            Piece pieceExpected = board.GetSquare("d3").GetPiece();
            Assert.AreNotEqual(pieceGiven, pieceExpected);
        }

        [UnityTest]
        public IEnumerator Capture_WhenEnPassant_PieceIsCaptured()
        {
            yield return InitGameWithPreset("4k3/8/8/8/2Pp4/8/8/4K3 b - c3 0 1");

            var board = Object.FindObjectOfType<Board>();
            var currentPlayer = Object.FindObjectOfType<Competitors>().CurrentPlayer as PlayerOffline;
            Piece pieceGiven = board.GetSquare("d4").GetPiece();
            Piece pieceToCapture = board.GetSquare("c4").GetPiece();

            currentPlayer?.Move("d4c3");
            yield return new WaitForSecondsRealtime(MoveSec);

            Piece pieceExpected = board.GetSquare("c3").GetPiece();
            Assert.AreEqual(pieceGiven, pieceExpected);
            Assert.IsNull(pieceToCapture.GetSquare());
        }

        private static IEnumerator InitGameWithPreset(string fenPreset)
        {
            var gameSettingsContainer = Object.FindObjectOfType<GameSettingsContainer>();
            gameSettingsContainer.SetCurrentFen(fenPreset);

            var gameInitialization = Object.FindObjectOfType<GameInitialization>();
            yield return gameInitialization.Init().ToCoroutine();
            yield return gameInitialization.StartGame().ToCoroutine();
        }

        [UnitySetUp]
        public IEnumerator RunGameTest_Setup()
        {
            yield return SceneManager.LoadSceneAsync("MainMenuScene");

            var gameSettingsContainerObj = Object.FindObjectOfType<GameSettingsContainer>();
            gameSettingsContainerObj.SetupGameOffline();

            yield return SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
            yield return SceneManager.UnloadSceneAsync("MainMenuScene");
        }
    }
}