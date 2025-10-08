using System.Collections;
using ChessBoard.Pieces;
using NUnit.Framework;
using UnityEngine.TestTools;

using static PlayTests.TestUtils;

namespace PlayTests
{
    [TestFixture]
    public class KnightTest
    {
        [UnitySetUp]
        public IEnumerator Setup()
        {
            yield return TestSetup();
        }

        [UnityTest]
        public IEnumerator Capture_WhenAllDirections_CanMoveToSquaresAndCapture()
        {
            yield return InitGameWithPreset("6k1/8/2r1p3/1n3q2/3N4/1b3b2/2p1r3/7K b - - 0 1");
            Piece pieceGiven = Board.GetSquare("d4").GetPiece();

            yield return MoveAndAssert("d4c2", pieceGiven);
            yield return MoveAndAssert("d4b3", pieceGiven);
            yield return MoveAndAssert("d4b5", pieceGiven);
            yield return MoveAndAssert("d4c6", pieceGiven);
            yield return MoveAndAssert("d4e6", pieceGiven);
            yield return MoveAndAssert("d4f5", pieceGiven);
            yield return MoveAndAssert("d4f3", pieceGiven);
            yield return MoveAndAssert("d4e2", pieceGiven);
        }

        [UnityTest]
        public IEnumerator Move_WhenAllDirections_CanMoveToSquares()
        {
            yield return InitGameWithPreset("P5k1/8/8/8/3N4/8/8/7K b - - 0 1");
            Piece pieceGiven = Board.GetSquare("d4").GetPiece();

            yield return MoveAndAssert("d4c2", pieceGiven);
            yield return MoveAndAssert("d4b3", pieceGiven);
            yield return MoveAndAssert("d4b5", pieceGiven);
            yield return MoveAndAssert("d4c6", pieceGiven);
            yield return MoveAndAssert("d4e6", pieceGiven);
            yield return MoveAndAssert("d4f5", pieceGiven);
            yield return MoveAndAssert("d4f3", pieceGiven);
            yield return MoveAndAssert("d4e2", pieceGiven);
        }


        [UnityTest]
        public IEnumerator Move_WhenHopOver_CanMoveToSquare()
        {
            yield return InitGameWithPreset("6k1/8/3P4/2rrQ3/1BRNqp2/2bRb3/3P4/7K b - - 0 1");
            Piece pieceGiven = Board.GetSquare("d4").GetPiece();

            yield return MoveAndAssert("d4c2", pieceGiven);
            yield return MoveAndAssert("d4b3", pieceGiven);
            yield return MoveAndAssert("d4b5", pieceGiven);
            yield return MoveAndAssert("d4c6", pieceGiven);
            yield return MoveAndAssert("d4e6", pieceGiven);
            yield return MoveAndAssert("d4f5", pieceGiven);
            yield return MoveAndAssert("d4f3", pieceGiven);
            yield return MoveAndAssert("d4e2", pieceGiven);
        }

        private IEnumerator MoveAndAssert(string where, Piece pieceGiven)
        {
            yield return Move(where);
            string square = where.Substring(2);
            Piece pieceExpected = Board.GetSquare(square).GetPiece();
            Assert.AreEqual(pieceExpected, pieceGiven);

            yield return Undo();
        }
    }
}