using ChessBoard;
using ChessBoard.Pieces;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Ui.Promotion
{
    public class ConcreteColorPromotion : MonoBehaviour
    {
        [FormerlySerializedAs("boardBuilder")]
        [SerializeField] private Board board;

        [Space]
        [SerializeField] private Button queenButton;
        [SerializeField] private Button rookButton;
        [SerializeField] private Button bishopButton;
        [SerializeField] private Button knightButton;

        private void OnEnable()
        {
            queenButton.onClick.AddListener(SelectQueen);
            rookButton.onClick.AddListener(SelectRook);
            bishopButton.onClick.AddListener(SelectBishop);
            knightButton.onClick.AddListener(SelectKnight);
        }

        private void OnDisable()
        {
            queenButton.onClick.RemoveListener(SelectQueen);
            rookButton.onClick.RemoveListener(SelectRook);
            bishopButton.onClick.RemoveListener(SelectBishop);
            knightButton.onClick.RemoveListener(SelectKnight);
        }

        private void SelectQueen() => board.Select(PieceType.Queen);
        private void SelectRook() => board.Select(PieceType.Rook);
        private void SelectBishop() => board.Select(PieceType.Bishop);
        private void SelectKnight() => board.Select(PieceType.Knight);
    }
}