using Chess3D.Runtime.Core.Logic;
using UnityEngine;

namespace Chess3D.Runtime.Bootstrap.Settings
{
    [System.Serializable]
    public class GameSettings
    {
        public PieceColor PlayerColor = PieceColor.White;

        public PlayerSettings Player1Settings = new() { Name = "Player 1" };
        public PlayerSettings Player2Settings = new() { Name = "Player 2" };

        [Header("Game Settings")]
        public Vector2Int Time = new(30, 0);
        public string CurrentFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public string DefaultFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public string SavedFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public string CustomFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        [Header("Qol Settings")]
        public bool IsAutoPromoteToQueen;
        public bool IsAutorotateCamera = true;
        public bool IsRotateCameraOnStart = true;

        [Header("Draw Rules")]
        public int FiftyMoveRuleCount = 50;
        public int ThreefoldRepetitionCount = 3;

        [Header("Models")]
        public string PiecesModelAddress;
        public string BoardModelAddress;
    }
}