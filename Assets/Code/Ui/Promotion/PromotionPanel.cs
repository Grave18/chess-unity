using Logic;
using UnityEngine;

namespace Ui.Promotion
{
    public class PromotionPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject whiteRoot;
        [SerializeField] private GameObject blackRoot;

        public void Show(PieceColor pieceColor)
        {
            if (pieceColor == PieceColor.White)
            {
                whiteRoot.SetActive(true);
            }
            else
            {
                blackRoot.SetActive(true);
            }
        }

        public void Hide()
        {
            whiteRoot.SetActive(false);
            blackRoot.SetActive(false);
        }
    }
}