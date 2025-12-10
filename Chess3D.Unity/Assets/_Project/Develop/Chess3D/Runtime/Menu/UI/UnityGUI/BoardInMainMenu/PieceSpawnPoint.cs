using Chess3D.Runtime.Core.ChessBoard.Info;
using Chess3D.Runtime.Core.Logic;
using UnityEngine;

namespace Chess3D.Runtime.Menu.UI.UnityGUI.BoardInMainMenu
{
    public class PieceSpawnPoint : MonoBehaviour
    {
        [SerializeField] private PieceType pieceType = PieceType.Pawn;
        [SerializeField] private PieceColor pieceColor = PieceColor.White;

        public PieceType PieceType => pieceType;
        public PieceColor PieceColor => pieceColor;
    }
}