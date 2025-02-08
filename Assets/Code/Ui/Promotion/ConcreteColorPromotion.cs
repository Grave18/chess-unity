using Board.Builder;
using Board.Pieces;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Promotion
{
    public class ConcreteColorPromotion : MonoBehaviour
    {
        [SerializeField] private BoardBuilder boardBuilder;

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

        private void SelectQueen() => boardBuilder.Select(PieceType.Queen);
        private void SelectRook() => boardBuilder.Select(PieceType.Rook);
        private void SelectBishop() => boardBuilder.Select(PieceType.Bishop);
        private void SelectKnight() => boardBuilder.Select(PieceType.Knight);
    }
}