using ChessGame.ChessBoard.Info;
using ChessGame.Logic;
using UnityEngine;

namespace BoardEditor
{
    public class PieceSpawnPoint : MonoBehaviour
    {
        [SerializeField] private PieceType pieceType = PieceType.Pawn;
        [SerializeField] private PieceColor pieceColor = PieceColor.White;

        public PieceType PieceType => pieceType;
        public PieceColor PieceColor => pieceColor;
    }
}