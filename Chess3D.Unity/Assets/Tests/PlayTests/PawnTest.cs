using System.Collections;
using ChessBoard;
using ChessBoard.Pieces;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using static PlayTests.TestUtils;
namespace PlayTests
{
    [TestFixture]
    public class PawnTest
    {
        [UnitySetUp]
        public IEnumerator RunGameTest_Setup()
        {
            yield return TestSetup();
        }

        [UnityTest]
        public IEnumerator Move_WhenOneSquare_CanMoveToSquare()
        {
            yield return InitGameWithPreset("3k4/8/8/8/8/8/3P4/4K3 w - - 0 1");

            Piece pieceGiven = GetPiece("d2");
            yield return Move_AssertEqual_Undo("d2d3", pieceGiven);
        }

        [UnityTest]
        public IEnumerator Move_WhenOneSquareAndUndo_PreserveIsFirstMove()
        {
            yield return InitGameWithPreset("3k4/8/8/8/8/8/3P4/4K3 w - - 0 1");

            Piece pieceGiven = GetPiece("d2");
            yield return Move("d2d3");
            yield return Undo();

            Assert.IsTrue(pieceGiven.IsFirstMove);
        }

        [UnityTest]
        public IEnumerator Move_WhenTwoSquaresIfFirstMove_CanMoveToToSquare()
        {
            yield return InitGameWithPreset("3k4/8/8/8/8/8/3P4/4K3 w - - 0 1");

            Piece pieceGiven = GetPiece("d2");
            yield return Move_AssertEqual_Undo("d2d4", pieceGiven);
        }

        [UnityTest]
        public IEnumerator Move_WhenTwoSquaresIfNotFirstMove_CanNotMoveToSquare()
        {
            yield return InitGameWithPreset("4k3/8/8/8/8/3P4/8/4K3 w - - 0 1");

            Piece pieceGiven = GetPiece("d3");
            yield return Move_AssertNotEqual_Undo("d3d5", pieceGiven);
        }

        [UnityTest]
        public IEnumerator Move_WhenThreeSquares_CanNotMoveToSquare()
        {
            yield return InitGameWithPreset("3k4/8/8/8/8/8/3P4/4K3 w - - 0 1");

            Piece pieceGiven = GetPiece("d2");
            yield return Move_AssertNotEqual_Undo("d2d5", pieceGiven);
        }

        [UnityTest]
        public IEnumerator Capture_WhenCapturedPieceIsLeftOrRight_CanMoveToSquareAndCapture()
        {
            yield return InitGameWithPreset("4k3/8/8/8/8/2ppp3/3P4/4K3 w - - 0 1");

            // Move Left
            Piece pieceGiven = GetPiece("d2");
            yield return Move_AssertEqual_Undo("d2c3", pieceGiven);

            // Move Right
            yield return Capture_AssertEqual_Undo("d2e3", pieceGiven);
        }

        [UnityTest]
        public IEnumerator Capture_WhenCapturedPieceIsFront_CanNotMoveToSquareAndCapture()
        {
            yield return InitGameWithPreset("4k3/8/8/8/8/2ppp3/3P4/4K3 w - - 0 1");

            Piece pieceGiven = GetPiece("d2");
            yield return Capture_AssertNotEqual_Undo("d2d3", pieceGiven);
        }

        [UnityTest]
        public IEnumerator Capture_WhenEnPassant_CanMoveToSquareAndCaptured()
        {
            yield return InitGameWithPreset("4k3/8/8/8/2Pp4/8/8/4K3 b - c3 0 1");

            Piece pieceGiven = GetPiece("d4");
            Piece pieceToCapture = GetPiece("c4");
            yield return Move("d4c3");

            Piece pieceExpected = GetPiece("c3");
            Assert.AreEqual(pieceGiven, pieceExpected);
            Assert.IsNull(pieceToCapture.GetSquare());
        }

        [UnityTest]
        public IEnumerator Promotion_WhenOnEmptySquare_CanPromoteToQueenWhenKnightWhenRookWhenBishop()
        {
            yield return InitGameWithPreset("7k/3P4/8/8/8/8/8/R6K w - - 0 1");

            var board = Object.FindObjectOfType<Board>();

            // Queen
            yield return Move("d7d8q");

            Piece pieceExpected = board.GetSquare("d8").GetPiece();
            Assert.IsTrue(pieceExpected is Queen, "Piece is not a Queen");

            yield return Undo();

            // Knight
            yield return Move("d7d8n");

            pieceExpected = board.GetSquare("d8").GetPiece();
            Assert.IsTrue(pieceExpected is Knight, "Piece is not a Knight");

            yield return Undo();

            // Rook
            yield return Move("d7d8r");

            pieceExpected = board.GetSquare("d8").GetPiece();
            Assert.IsTrue(pieceExpected is Rook, "Piece is not a Rook");

            yield return Undo();

            // Bishop
            yield return Move("d7d8b");

            pieceExpected = board.GetSquare("d8").GetPiece();
            Assert.IsTrue(pieceExpected is Bishop, "Piece is not a Bishop");
        }

        [UnityTest]
        public IEnumerator Promotion_WhenCapture_CanCaptureAndPromoteToQueen()
        {
            yield return InitGameWithPreset("2r4k/3P4/8/8/8/8/8/R6K w - - 0 1");

            var board = Object.FindObjectOfType<Board>();
            Piece pieceToCapture = board.GetSquare("c8").GetPiece();

            yield return Move("d7c8q");

            Piece pieceExpected = board.GetSquare("c8").GetPiece();
            Assert.IsTrue(pieceExpected is Queen, "Piece is not a Queen");
            Assert.IsTrue(pieceToCapture.GetSquare() is null, "Piece is not captured");
        }
    }
}