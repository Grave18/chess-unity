using Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class CurrentPlayerPointerPanel : MonoBehaviour
    {
        [SerializeField] private Game game;
        [SerializeField] private Image whiteImage;
        [SerializeField] private Image blackImage;

        private void OnEnable()
        {
            game.OnEndTurn += OnEndTurn;
        }

        private void OnEndTurn()
        {
            if (game.CurrentTurnColor == PieceColor.White)
            {

                whiteImage.CrossFadeAlpha(1f, 0.2f, false);
                blackImage.CrossFadeAlpha(0f, 0.2f, false);
            }
            else if (game.CurrentTurnColor == PieceColor.Black)
            {
                whiteImage.CrossFadeAlpha(0f, 0.2f, false);
                blackImage.CrossFadeAlpha(1f, 0.2f, false);
            }
        }
    }
}