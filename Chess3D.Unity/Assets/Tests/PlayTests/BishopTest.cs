using System.Collections;
using ChessBoard.Pieces;
using NUnit.Framework;
using UnityEngine.TestTools;

using static PlayTests.TestUtils;

namespace PlayTests
{
    [TestFixture]
    public class BishopTest
    {
        [UnitySetUp]
        public IEnumerator Setup()
        {
            yield return TestSetup();
        }

        [UnityTest]
        public IEnumerator Move_WhenAllDirectionsToEndOfBoard_CanMoveToSquares()
        {
            yield return InitGameWithPreset("6k1/8/8/8/3b3p/8/8/7K b - - 0 1");
            Piece pieceGiven = Board.GetSquare("d4").GetPiece();

            yield return Move_AssertEqual_Undo("d4a7", pieceGiven);
            yield return Move_AssertEqual_Undo("d4h8", pieceGiven);
            yield return Move_AssertEqual_Undo("d4g1", pieceGiven);
            yield return Move_AssertEqual_Undo("d4a1", pieceGiven);
        }

        [UnityTest]
        public IEnumerator Move_WhenAllDirectionsThroughPieces_CanNotMoveToSquares()
        {
            yield return InitGameWithPreset("6k1/8/1P3p2/8/3b4/8/1p3P2/7K b - - 0 1");
            Piece pieceGiven = Board.GetSquare("d4").GetPiece();

            yield return Move_AssertNotEqual_Undo("d4a7", pieceGiven);
            yield return Move_AssertNotEqual_Undo("d4h8", pieceGiven);
            yield return Move_AssertNotEqual_Undo("d4g1", pieceGiven);
            yield return Move_AssertNotEqual_Undo("d4a1", pieceGiven);
        }

        [UnityTest]
        public IEnumerator Capture_WhenAllDirections_CanMoveToSquaresAndCapture()
        {
            yield return InitGameWithPreset("6k1/8/1P3P2/8/3b4/8/1P3P2/7K b - - 0 1");
            Piece pieceGiven = Board.GetSquare("d4").GetPiece();

            yield return Capture_AssertEqual_Undo("d4b2", pieceGiven);
            yield return Capture_AssertEqual_Undo("d4b6", pieceGiven);
            yield return Capture_AssertEqual_Undo("d4f6", pieceGiven);
            yield return Capture_AssertEqual_Undo("d4f2", pieceGiven);
        }

        [UnityTest]
        public IEnumerator Capture_WhenAllDirectionsSameColorPiece_CanNotMoveToSquare()
        {
            yield return InitGameWithPreset("6k1/8/1p3p2/8/3b4/8/1p3p2/7K b - - 0 1");
            Piece pieceGiven = Board.GetSquare("d4").GetPiece();

            yield return Capture_AssertNotEqual_Undo("d4b2", pieceGiven);
            yield return Capture_AssertNotEqual_Undo("d4b6", pieceGiven);
            yield return Capture_AssertNotEqual_Undo("d4f6", pieceGiven);
            yield return Capture_AssertNotEqual_Undo("d4f2", pieceGiven);
        }
    }
}