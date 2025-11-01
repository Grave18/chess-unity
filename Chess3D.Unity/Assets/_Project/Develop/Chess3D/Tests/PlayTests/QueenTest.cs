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
        public IEnumerator Move_WhenAllDirectionsToEndOfBoard_CanMoveToSquares()
        {
            yield return InitGameWithPreset("6k1/8/8/8/3q4/8/8/7K b - - 0 1");
            Piece pieceGiven = Board.GetSquare("d4").GetPiece();

            yield return Move_AssertEqual_Undo("d4a4", pieceGiven);
            yield return Move_AssertEqual_Undo("d4a7", pieceGiven);
            yield return Move_AssertEqual_Undo("d4d8", pieceGiven);
            yield return Move_AssertEqual_Undo("d4h8", pieceGiven);
            yield return Move_AssertEqual_Undo("d4h4", pieceGiven);
            yield return Move_AssertEqual_Undo("d4g1", pieceGiven);
            yield return Move_AssertEqual_Undo("d4d1", pieceGiven);
            yield return Move_AssertEqual_Undo("d4a1", pieceGiven);
        }

        [UnityTest]
        public IEnumerator Move_WhenAllDirectionsThroughPieces_CanNotMoveToSquares()
        {
            yield return InitGameWithPreset("6k1/8/1P1P1P2/8/1p1q1P2/8/1p1p1p2/7K b - - 0 1");
            Piece pieceGiven = Board.GetSquare("d4").GetPiece();

            yield return Move_AssertNotEqual_Undo("d4a4", pieceGiven);
            yield return Move_AssertNotEqual_Undo("d4a7", pieceGiven);
            yield return Move_AssertNotEqual_Undo("d4d8", pieceGiven);
            yield return Move_AssertNotEqual_Undo("d4h8", pieceGiven);
            yield return Move_AssertNotEqual_Undo("d4h4", pieceGiven);
            yield return Move_AssertNotEqual_Undo("d4g1", pieceGiven);
            yield return Move_AssertNotEqual_Undo("d4d1", pieceGiven);
            yield return Move_AssertNotEqual_Undo("d4a1", pieceGiven);
        }

        [UnityTest]
        public IEnumerator Capture_WhenAllDirections_CanMoveToSquaresAndCapture()
        {
            yield return InitGameWithPreset("6k1/8/1P1P1P2/8/1P1q1P2/8/1P1P1P2/7K b - - 0 1");
            Piece pieceGiven = Board.GetSquare("d4").GetPiece();

            yield return Capture_AssertEqual_Undo("d4b4", pieceGiven);
            yield return Capture_AssertEqual_Undo("d4b6", pieceGiven);
            yield return Capture_AssertEqual_Undo("d4d6", pieceGiven);
            yield return Capture_AssertEqual_Undo("d4f6", pieceGiven);
            yield return Capture_AssertEqual_Undo("d4f4", pieceGiven);
            yield return Capture_AssertEqual_Undo("d4f2", pieceGiven);
            yield return Capture_AssertEqual_Undo("d4d2", pieceGiven);
            yield return Capture_AssertEqual_Undo("d4b2", pieceGiven);
        }

        [UnityTest]
        public IEnumerator Capture_WhenCaptureSameColorPiece_CanNotMoveToSquareAndCapture()
        {
            yield return InitGameWithPreset("6k1/8/1p1p1p2/8/1p1q1p2/8/1p1p1p2/7K b - - 0 1");
            Piece pieceGiven = Board.GetSquare("d4").GetPiece();

            yield return Capture_AssertNotEqual_Undo("d4b4", pieceGiven);
            yield return Capture_AssertNotEqual_Undo("d4b6", pieceGiven);
            yield return Capture_AssertNotEqual_Undo("d4d6", pieceGiven);
            yield return Capture_AssertNotEqual_Undo("d4f6", pieceGiven);
            yield return Capture_AssertNotEqual_Undo("d4f4", pieceGiven);
            yield return Capture_AssertNotEqual_Undo("d4f2", pieceGiven);
            yield return Capture_AssertNotEqual_Undo("d4d2", pieceGiven);
            yield return Capture_AssertNotEqual_Undo("d4b2", pieceGiven);
        }
    }
}