using System.Collections;
using ChessBoard.Pieces;
using NUnit.Framework;
using UnityEngine.TestTools;

using static PlayTests.TestUtils;

namespace PlayTests
{
    [TestFixture]
    public class QueenTest
    {
        [UnitySetUp]
        public IEnumerator Setup()
        {
            yield return TestSetup();
        }

        [UnityTest]
        public IEnumerator Move_WhenAllDirections_CanMoveToSquares()
        {
            yield return InitGameWithPreset("6k1/8/8/8/3q4/8/8/7K b - - 0 1");
            Piece pieceGiven = Board.GetSquare("d4").GetPiece();

            yield return Move("d4a4");
            Piece pieceExpected = Board.GetSquare("a4").GetPiece();
            Assert.AreEqual(pieceExpected, pieceGiven);

            yield return Undo();

            yield return Move("d4a7");
            pieceExpected = Board.GetSquare("a7").GetPiece();
            Assert.AreEqual(pieceExpected, pieceGiven);

            yield return Undo();

            yield return Move("d4d8");
            pieceExpected = Board.GetSquare("d8").GetPiece();
            Assert.AreEqual(pieceExpected, pieceGiven);

            yield return Undo();

            yield return Move("d4h8");
            pieceExpected = Board.GetSquare("h8").GetPiece();
            Assert.AreEqual(pieceExpected, pieceGiven);

            yield return Undo();

            yield return Move("d4h4");
            pieceExpected = Board.GetSquare("h4").GetPiece();
            Assert.AreEqual(pieceExpected, pieceGiven);

            yield return Undo();

            yield return Move("d4g1");
            pieceExpected = Board.GetSquare("g1").GetPiece();
            Assert.AreEqual(pieceExpected, pieceGiven);

            yield return Undo();

            yield return Move("d4d1");
            pieceExpected = Board.GetSquare("d1").GetPiece();
            Assert.AreEqual(pieceExpected, pieceGiven);

            yield return Undo();

            yield return Move("d4a1");
            pieceExpected = Board.GetSquare("a1").GetPiece();
            Assert.AreEqual(pieceExpected, pieceGiven);
        }

        [UnityTest]
        public IEnumerator Move_WhenThroughPiece_CanNotMoveToSquares()
        {
            yield return InitGameWithPreset("6k1/8/8/8/1R1q4/8/5r2/7K b - - 0 1");
            Piece pieceGiven = Board.GetSquare("d4").GetPiece();

            yield return Move("d4a4");
            Piece pieceExpected = Board.GetSquare("a4").GetPiece();
            Assert.AreNotEqual(pieceExpected, pieceGiven);

            yield return Undo();

            yield return Move("d4g1");
            pieceExpected = Board.GetSquare("g1").GetPiece();
            Assert.AreNotEqual(pieceExpected, pieceGiven);
        }

        [UnityTest]
        public IEnumerator Move_WhenCaptureSameColorPiece_CanNotMoveToSquare()
        {
            yield return InitGameWithPreset("6k1/8/8/8/1r1q4/8/5R2/7K b - - 0 1");
            Piece pieceGiven = Board.GetSquare("d4").GetPiece();

            yield return Move("d4b4");
            Piece pieceExpected = Board.GetSquare("b4").GetPiece();
            Assert.AreNotEqual(pieceExpected, pieceGiven);
        }

        [UnityTest]
        public IEnumerator Move_WhenCapture_CanMoveToSquareAndPieceCaptured()
        {
            yield return InitGameWithPreset("6k1/8/8/8/1r1q4/8/5R2/7K b - - 0 1");
            Piece pieceGiven = Board.GetSquare("d4").GetPiece();
            Piece capturedPiece = Board.GetSquare("f2").GetPiece();

            yield return Move("d4f2");
            Piece pieceExpected = Board.GetSquare("f2").GetPiece();
            Assert.AreEqual(pieceExpected, pieceGiven);
            Assert.IsNull(capturedPiece.GetSquare(), "Piece is not captured");
        }
    }
}