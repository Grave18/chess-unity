using System.Collections;
using ChessBoard;
using ChessBoard.Pieces;
using Logic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using static PlayTests.TestUtils;

namespace PlayTests
{
    [TestFixture]
    public class KingTest
    {
        [UnitySetUp]
        public IEnumerator Setup()
        {
            yield return TestSetup();
        }

        [UnityTest]
        public IEnumerator Move_WhenAllDirections_CanMoveToSquare()
        {
            yield return InitGameWithPreset("4k2r/8/8/8/3K4/8/8/7R w - - 0 1");

            var board = Object.FindObjectOfType<Board>();
            Piece pieceGiven = board.GetSquare("d4").GetPiece();

            yield return Move_AssertEqual_Undo("d4e3", pieceGiven);
            yield return Move_AssertEqual_Undo("d4e4", pieceGiven);
            yield return Move_AssertEqual_Undo("d4e5", pieceGiven);
            yield return Move_AssertEqual_Undo("d4d5", pieceGiven);
            yield return Move_AssertEqual_Undo("d4c5", pieceGiven);
            yield return Move_AssertEqual_Undo("d4c4", pieceGiven);
            yield return Move_AssertEqual_Undo("d4c3", pieceGiven);
            yield return Move_AssertEqual_Undo("d4d3", pieceGiven);
        }

        [UnityTest]
        public IEnumerator Move_WhenTwoSquares_IsNotMovedToSquare()
        {
            yield return InitGameWithPreset("4k3/8/8/8/8/2P5/8/4K3 w - - 0 1");

            var board = Object.FindObjectOfType<Board>();
            Piece pieceGiven = board.GetSquare("e1").GetPiece();

            yield return Move_AssertNotEqual_Undo("e1e3", pieceGiven);
        }

        [UnityTest]
        public IEnumerator Move_WhenMoveOnAttackedSquare_CanNotMoveToSquare()
        {
            yield return InitGameWithPreset("4k3/b7/8/8/3K4/8/8/8 w - - 0 1");

            var board = Object.FindObjectOfType<Board>();
            Piece pieceGiven = board.GetSquare("d4").GetPiece();

            yield return Move_AssertNotEqual_Undo("d4c5", pieceGiven);
            yield return Move_AssertNotEqual_Undo("d4e3", pieceGiven);
        }

        [UnityTest]
        public IEnumerator Move_WhenMoveOnAttackedButDefendedSquare_CanMoveToSquare()
        {
            yield return InitGameWithPreset("4k3/b7/1P6/8/3K4/8/8/8 w - - 0 1");

            var board = Object.FindObjectOfType<Board>();
            Piece pieceGiven = board.GetSquare("d4").GetPiece();

            yield return Move_AssertEqual_Undo("d4c5", pieceGiven);
            yield return Move_AssertEqual_Undo("d4e3", pieceGiven);
        }

        [UnityTest]
        public IEnumerator Capture_WhenSideDirections_CanMoveToSquareAndCapture()
        {
            yield return InitGameWithPreset("4k2r/8/8/2p1p3/2pKp3/2p1p3/8/7R w - - 0 1");

            var board = Object.FindObjectOfType<Board>();
            Piece pieceGiven = board.GetSquare("d4").GetPiece();

            yield return Capture_AssertEqual_Undo("d4e3", pieceGiven);
            yield return Capture_AssertEqual_Undo("d4e4", pieceGiven);
            yield return Capture_AssertEqual_Undo("d4e5", pieceGiven);
            yield return Capture_AssertEqual_Undo("d4c5", pieceGiven);
            yield return Capture_AssertEqual_Undo("d4c4", pieceGiven);
            yield return Capture_AssertEqual_Undo("d4c3", pieceGiven);
        }

        [UnityTest]
        public IEnumerator Capture_WhenUpAndDownDirections_CanMoveToSquareAndCapture()
        {
            yield return InitGameWithPreset("4k2r/8/8/3p4/3K4/3p4/8/7R w - - 0 1");

            var board = Object.FindObjectOfType<Board>();
            Piece pieceGiven = board.GetSquare("d4").GetPiece();

            yield return Capture_AssertEqual_Undo("d4d5", pieceGiven);
            yield return Capture_AssertEqual_Undo("d4d3", pieceGiven);
        }

        [UnityTest]
        public IEnumerator Check_WhenUnderAttack_StateIsCheck()
        {
            yield return InitGameWithPreset("4k3/8/8/8/8/5r2/8/4K3 b - - 0 1");

            var game = Object.FindObjectOfType<Game>();

            Assert.AreEqual(CheckType.None, game.CheckType);

            yield return Move("f3e3");

            Assert.AreEqual(CheckType.Check, game.CheckType);
        }

        [UnityTest]
        public IEnumerator Check_WhenAttackGone_StateIsNone()
        {
            yield return InitGameWithPreset("4k3/8/8/8/8/4r3/8/4K3 w - - 0 1");

            var game = Object.FindObjectOfType<Game>();

            Assert.AreEqual(CheckType.Check, game.CheckType);

            yield return Move("e1f1");

            Assert.AreEqual(CheckType.None, game.CheckType);
        }

        [UnityTest]
        public IEnumerator CheckMate_WhenUnderAttackAndCanNotMove_StateIsCheckMate()
        {
            yield return InitGameWithPreset("6k1/8/8/8/8/4r3/r7/1PKP4 b - - 0 1");

            yield return Move("e3c3");

            Assert.AreEqual(CheckType.Checkmate, TestUtils.Game.CheckType);
        }

        [UnityTest]
        public IEnumerator Stalemate_WhenKingPinned_StateIsStalemate()
        {
            yield return InitGameWithPreset("4k3/8/8/8/6q1/8/8/7K b - - 0 1");

            var game = Object.FindObjectOfType<Game>();

            yield return Move("g4g3");

            Assert.AreEqual(game.CheckType, CheckType.Draw);
        }

        [UnityTest]
        public IEnumerator Castling_WhenCanCastleAndCastling_MoveToSquare()
        {
            yield return InitGameWithPreset("4k3/8/8/8/8/8/8/4K2R w K - 0 1");

            var game = Object.FindObjectOfType<Game>();

            Piece kingGiven = game.Board.GetSquare("e1").GetPiece();
            Piece rookGiven = game.Board.GetSquare("h1").GetPiece();

            yield return Move("e1g1");

            Piece kingExpected = game.Board.GetSquare("g1").GetPiece();
            Piece rookExpected = game.Board.GetSquare("f1").GetPiece();
            Assert.AreEqual(kingExpected, kingGiven);
            Assert.AreEqual(rookExpected, rookGiven);
        }

        [UnityTest]
        public IEnumerator Castling_WhenUnderAttack_CanNotMoveToSquare()
        {
            yield return InitGameWithPreset("4k3/8/8/8/8/4r3/8/4K2R w K - 0 1");

            var game = Object.FindObjectOfType<Game>();

            Piece kingGiven = game.Board.GetSquare("e1").GetPiece();
            Piece rookGiven = game.Board.GetSquare("h1").GetPiece();

            yield return Move("e1g1");

            Piece kingExpected = game.Board.GetSquare("g1").GetPiece();
            Piece rookExpected = game.Board.GetSquare("f1").GetPiece();
            Assert.AreNotEqual(kingExpected, kingGiven);
            Assert.AreNotEqual(rookExpected, rookGiven);
        }

        [UnityTest]
        public IEnumerator Castling_WhenMoveLineUnderAttack_CanNotMoveToSquare()
        {
            yield return InitGameWithPreset("4k3/8/8/8/8/5r2/8/4K2R w K - 0 1");

            var game = Object.FindObjectOfType<Game>();

            Piece kingGiven = game.Board.GetSquare("e1").GetPiece();
            Piece rookGiven = game.Board.GetSquare("h1").GetPiece();

            yield return Move("e1g1");

            Piece kingExpected = game.Board.GetSquare("g1").GetPiece();
            Piece rookExpected = game.Board.GetSquare("f1").GetPiece();
            Assert.AreNotEqual(kingExpected, kingGiven);
            Assert.AreNotEqual(rookExpected, rookGiven);
        }
    }
}