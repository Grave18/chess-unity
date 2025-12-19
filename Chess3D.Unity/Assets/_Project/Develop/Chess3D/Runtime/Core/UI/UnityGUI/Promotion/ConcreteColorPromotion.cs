using Chess3D.Runtime.Core.ChessBoard.Info;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Chess3D.Runtime.Core.UI.UnityGUI.Promotion
{
    public class ConcreteColorPromotion : MonoBehaviour
    {
        [FormerlySerializedAs("promotionPanel")]
        [SerializeField] private PromotionView promotionView;

        [Space]
        [SerializeField] private Button queenButton;
        [SerializeField] private Button rookButton;
        [SerializeField] private Button bishopButton;
        [SerializeField] private Button knightButton;

        private void Awake()
        {
            queenButton.OnClickAsObservable().Subscribe(_ => promotionView.Select(PieceType.Queen)).AddTo(this);
            rookButton.OnClickAsObservable().Subscribe(_ => promotionView.Select(PieceType.Rook)).AddTo(this);
            bishopButton.OnClickAsObservable().Subscribe(_ => promotionView.Select(PieceType.Bishop)).AddTo(this);
            knightButton.OnClickAsObservable().Subscribe(_ => promotionView.Select(PieceType.Knight)).AddTo(this);
        }
    }
}