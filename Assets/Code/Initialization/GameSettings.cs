using ChessGame.Logic;
using UnityEngine;

namespace Initialization
{
    [System.Serializable]
    public class GameSettings
    {

        public PieceColor PlayerColor = PieceColor.White;

        public PlayerSettings Player1Settings = new();
        public PlayerSettings Player2Settings = new();

        [Space]
        public Vector2 Time;
        public string CurrentFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public string DefaultFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public string SavedFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        [Space]
        public bool IsAutoPromoteToQueen;
    }
}