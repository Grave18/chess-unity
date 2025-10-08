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
    public class PawnTest
    {
        private const float MoveSec = 2f;
        private const float UndoSec = 1f;

        [UnityTest]
        public IEnumerator Move_WhenOneSquare_PieceIsMovedToToSquare()
        {
            yield return TestUtils.InitGameWithPreset("3k4/8/8/8/8/8/3P4/4K3 w - - 0 1");

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
            yield return TestUtils.InitGameWithPreset("3k4/8/8/8/8/8/3P4/4K3 w - - 0 1");

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
            yield return TestUtils.InitGameWithPreset("4k3/8/8/8/8/3P4/8/4K3 w - - 0 1");

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
            yield return TestUtils.InitGameWithPreset("3k4/8/8/8/8/8/3P4/4K3 w - - 0 1");

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
            yield return TestUtils.InitGameWithPreset("4k3/8/8/8/8/2ppp3/3P4/4K3 w - - 0 1");

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
            yield return TestUtils.InitGameWithPreset("4k3/8/8/8/8/2ppp3/3P4/4K3 w - - 0 1");

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
            yield return TestUtils.InitGameWithPreset("4k3/8/8/8/2Pp4/8/8/4K3 b - c3 0 1");

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

        [UnityTest]
        public IEnumerator Promotion_WhenOnEmptySquare_PawnIsPromotedToQueen_Knight_Rook_Bishop()
        {
            yield return TestUtils.InitGameWithPreset("7k/3P4/8/8/8/8/8/R6K w - - 0 1");

            var game = Object.FindObjectOfType<Game>();
            var board = Object.FindObjectOfType<Board>();
            var currentPlayer = Object.FindObjectOfType<Competitors>().CurrentPlayer as PlayerOffline;

            // Queen
            yield return TestUtils.Move("d7d8q");

            Piece pieceExpected = board.GetSquare("d8").GetPiece();
            Assert.IsTrue(pieceExpected is Queen, "Piece is not a Queen");

            yield return TestUtils.Undo();

            // Knight
            yield return TestUtils.Move("d7d8n");

            pieceExpected = board.GetSquare("d8").GetPiece();
            Assert.IsTrue(pieceExpected is Knight, "Piece is not a Knight");

            yield return TestUtils.Undo();

            // Rook
            yield return TestUtils.Move("d7d8r");

            pieceExpected = board.GetSquare("d8").GetPiece();
            Assert.IsTrue(pieceExpected is Rook, "Piece is not a Rook");

            yield return TestUtils.Undo();

            // Bishop
            yield return TestUtils.Move("d7d8b");

            pieceExpected = board.GetSquare("d8").GetPiece();
            Assert.IsTrue(pieceExpected is Bishop, "Piece is not a Bishop");
        }

        [UnityTest]
        public IEnumerator Promotion_WhenCapture_CaptureAndPawnIsPromotedToQueen()
        {
            yield return TestUtils.InitGameWithPreset("2r4k/3P4/8/8/8/8/8/R6K w - - 0 1");

            var game = Object.FindObjectOfType<Game>();
            var board = Object.FindObjectOfType<Board>();
            var currentPlayer = Object.FindObjectOfType<Competitors>().CurrentPlayer as PlayerOffline;
            Piece pieceToCapture = board.GetSquare("c8").GetPiece();

            yield return TestUtils.Move("d7c8q");

            Piece pieceExpected = board.GetSquare("c8").GetPiece();
            Assert.IsTrue(pieceExpected is Queen, "Piece is not a Queen");
            Assert.IsTrue(pieceToCapture.GetSquare() is null, "Piece is not captured");
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