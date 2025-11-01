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

namespace PlayTests
{
    public static class TestUtils
    {
        public static Game Game => Object.FindObjectOfType<Game>();
        public static IPlayer Player => Object.FindObjectOfType<Competitors>().CurrentPlayer;
        public static Board Board => Object.FindObjectOfType<Board>();
        public static GameSettingsContainer GameSettingsContainer => Object.FindObjectOfType<GameSettingsContainer>();

        public static IEnumerator TestSetup()
        {
            yield return SceneManager.LoadSceneAsync("MainMenuScene");

            GameSettingsContainer.SetupGameHumanVsHumanOffline();

            yield return SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
            yield return SceneManager.UnloadSceneAsync("MainMenuScene");
        }

        public static IEnumerator InitGameWithPreset(string fenPreset, bool isAutoRotateCamera = false, bool isRotateCameraOnStart = false)
        {
            var gameSettingsContainer = Object.FindObjectOfType<GameSettingsContainer>();
            gameSettingsContainer.SetCurrentFen(fenPreset);
            gameSettingsContainer.IsAutoRotateCamera = isAutoRotateCamera;
            gameSettingsContainer.IsRotateCameraOnStart = isRotateCameraOnStart;

            var gameInitialization = Object.FindObjectOfType<GameInitialization>();
            yield return gameInitialization.Init().ToCoroutine();
            yield return gameInitialization.StartGame().ToCoroutine();
        }

        public static Piece GetPiece(string name)
        {
            var board = Object.FindObjectOfType<Board>();
            return board.GetSquare(name).GetPiece();
        }

        public static IEnumerator Move_AssertEqual_Undo(string where, Piece pieceGiven)
        {
            yield return Move(where);
            string square = where.Substring(2);
            Piece pieceExpected = Board.GetSquare(square).GetPiece();
            Assert.AreEqual(pieceExpected, pieceGiven);

            yield return Undo();
        }

        public static IEnumerator Capture_AssertEqual_Undo(string where, Piece pieceGiven)
        {
            string square = where.Substring(2);
            Piece pieceToCapture = Board.GetSquare(square).GetPiece();

            yield return Move(where);
            Piece pieceExpected = Board.GetSquare(square).GetPiece();
            Assert.AreEqual(pieceExpected, pieceGiven);
            Assert.Null(pieceToCapture.GetSquare());

            yield return Undo();
        }

        public static IEnumerator Move_AssertNotEqual_Undo(string where, Piece pieceGiven)
        {
            yield return Move(where);
            string square = where.Substring(2);
            Piece pieceExpected = Board.GetSquare(square).GetPiece();
            Assert.AreNotEqual(pieceExpected, pieceGiven);

            yield return Undo();
        }

        public static IEnumerator Capture_AssertNotEqual_Undo(string where, Piece pieceGiven)
        {
            string square = where.Substring(2);
            Piece pieceToCapture = Board.GetSquare(square).GetPiece();

            yield return Move(where);
            Piece pieceExpected = Board.GetSquare(square).GetPiece();
            Assert.AreNotEqual(pieceExpected, pieceGiven);
            Assert.NotNull(pieceToCapture.GetSquare());

            yield return Undo();
        }

        public static IEnumerator Move(string uci)
        {
            Game.GameStateMachine.Move(uci);
            return new WaitUntil(() => Game.GameStateMachine.StateName is "Idle" or "End Game");
        }

        public static IEnumerator Undo()
        {
            Game.GameStateMachine.Undo();
            return new WaitUntil(() => Game.GameStateMachine.StateName is "Idle" or "End Game");
        }

        public static IEnumerator Wait(float sec, bool isRealtime = true)
        {
            if (isRealtime)
            {
                yield return new WaitForSecondsRealtime(sec);
            }
            else
            {
                yield return new WaitForSeconds(sec);
            }
        }
    }
}