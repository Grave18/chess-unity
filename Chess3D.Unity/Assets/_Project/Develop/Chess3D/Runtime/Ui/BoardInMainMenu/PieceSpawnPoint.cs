using Chess3D.Runtime.ChessBoard.Info;
using Chess3D.Runtime.Logic;
using UnityEngine;

namespace Ui.BoardInMainMenu
{
    public class PieceSpawnPoint : MonoBehaviour
    {
        [SerializeField] private PieceType pieceType = PieceType.Pawn;
        [SerializeField] private PieceColor pieceColor = PieceColor.White;

        public PieceType PieceType => pieceType;
        public PieceColor PieceColor => pieceColor;
    }
}