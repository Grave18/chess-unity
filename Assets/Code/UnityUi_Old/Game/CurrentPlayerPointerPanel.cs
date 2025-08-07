using UnityEngine;
using UnityEngine.UI;

namespace Ui.Game
{
    public class CurrentPlayerPointerPanel : MonoBehaviour
    {
        [SerializeField] private Logic.Game game;
        [SerializeField] private Image whiteImage;
        [SerializeField] private Image blackImage;

        private void OnEnable()
        {
            game.OnStart += Fade;
            game.OnEndMove += Fade;
        }

        private void OnDisable()
        {
            game.OnStart -= Fade;
            game.OnEndMove -= Fade;
        }

        private void Fade()
        {
            if (game.IsWhiteTurn)
            {
                whiteImage.CrossFadeAlpha(1f, 0.2f, false);
                blackImage.CrossFadeAlpha(0f, 0.2f, false);
            }
            else if (game.IsBlackTurn)
            {
                whiteImage.CrossFadeAlpha(0f, 0.2f, false);
                blackImage.CrossFadeAlpha(1f, 0.2f, false);
            }
        }
    }
}