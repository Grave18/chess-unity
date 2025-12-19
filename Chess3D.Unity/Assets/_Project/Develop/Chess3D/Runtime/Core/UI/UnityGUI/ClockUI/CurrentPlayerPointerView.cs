using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Chess3D.Runtime.Core.UI.UnityGUI.ClockUI
{
    public sealed class CurrentPlayerPointerView : MonoBehaviour
    {
        private Logic.Game _game;
        [SerializeField] private Image whiteImage;
        [SerializeField] private Image blackImage;

        private readonly CompositeDisposable _disposables = new();

        public void Construct(Logic.Game game, CoreEvents coreEvents)
        {
            _game = game;

            coreEvents.OnStart.Subscribe(_ => Fade()).AddTo(_disposables);
            coreEvents.OnEndMove.Subscribe(_ => Fade()).AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
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