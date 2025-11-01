using UnityEngine;
using UnityEngine.UI;

namespace Chess3D.Runtime.UnityUi.InGame
{
    public class CurrentPlayerPointerPanel : MonoBehaviour
    {
        private Logic.Game _game;
        [SerializeField] private Image whiteImage;
        [SerializeField] private Image blackImage;


        public void Init(Logic.Game game)
        {
            _game = game;

            _game.OnStart += Fade;
            _game.OnEndMove += Fade;
        }

        private void OnDestroy()
        {
            if (_game == null)
            {
                return;
            }
            _game.OnStart -= Fade;
            _game.OnEndMove -= Fade;
        }

        private void Fade()
        {
            if (_game.IsWhiteTurn)
            {
                whiteImage.CrossFadeAlpha(1f, 0.2f, false);
                blackImage.CrossFadeAlpha(0f, 0.2f, false);
            }
            else if (_game.IsBlackTurn)
            {
                whiteImage.CrossFadeAlpha(0f, 0.2f, false);
                blackImage.CrossFadeAlpha(1f, 0.2f, false);
            }
        }
    }
}