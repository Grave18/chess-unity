using UnityEngine;
using UnityEngine.UI;

namespace Chess3D.Runtime.Core.Ui
{
    public class CurrentPlayerPointerPanel : MonoBehaviour
    {
        private Logic.Game _game;
        private CoreEvents _coreEvents;
        [SerializeField] private Image whiteImage;
        [SerializeField] private Image blackImage;


        public void Construct(Logic.Game game, CoreEvents coreEvents)
        {
            _game = game;
            _coreEvents = coreEvents;

            _coreEvents.OnStart += Fade;
            _coreEvents.OnEndMove += Fade;
        }

        private void OnDestroy()
        {
            if (_coreEvents == null)
            {
                return;
            }
            _coreEvents.OnStart -= Fade;
            _coreEvents.OnEndMove -= Fade;
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