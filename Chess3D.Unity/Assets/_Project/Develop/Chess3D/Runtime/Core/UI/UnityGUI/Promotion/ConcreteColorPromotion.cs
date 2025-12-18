using Chess3D.Runtime.Core.ChessBoard.Info;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Chess3D.Runtime.Core.UI.UnityGUI.Promotion
{
    public class ConcreteColorPromotion : MonoBehaviour
    {
        [FormerlySerializedAs("promotionPanel")] [SerializeField] private PromotionView promotionView;

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

        private void SelectQueen() => promotionView.Select(PieceType.Queen);
        private void SelectRook() => promotionView.Select(PieceType.Rook);
        private void SelectBishop() => promotionView.Select(PieceType.Bishop);
        private void SelectKnight() => promotionView.Select(PieceType.Knight);
    }
}