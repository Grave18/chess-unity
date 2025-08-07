using ChessBoard.Info;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Game.Promotion
{
    public class ConcreteColorPromotion : MonoBehaviour
    {
        [SerializeField] private PromotionPanel promotionPanel;

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

        private void SelectQueen() => promotionPanel.Select(PieceType.Queen);
        private void SelectRook() => promotionPanel.Select(PieceType.Rook);
        private void SelectBishop() => promotionPanel.Select(PieceType.Bishop);
        private void SelectKnight() => promotionPanel.Select(PieceType.Knight);
    }
}