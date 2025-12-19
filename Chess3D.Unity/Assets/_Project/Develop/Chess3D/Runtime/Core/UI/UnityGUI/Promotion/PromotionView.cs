using System.Collections;
using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.ChessBoard.Info;
using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Utilities.Common.Mathematics;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace Chess3D.Runtime.Core.UI.UnityGUI.Promotion
{
    public class PromotionView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject whiteRoot;
        [SerializeField] private GameObject blackRoot;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Settings")]
        [SerializeField] private float appearTimeSec = 0.2f;

        private Game _game;
        private PieceType _promotedPieceType;

        [Inject]
        public void Construct(Game game, CoreEvents coreEvents)
        {
            _game = game;

            coreEvents.RequestPromotion.Subscribe(RequestPromotedPiece).AddTo(this);
        }

        public void Select(PieceType pieceType)
        {
            _promotedPieceType = pieceType;
        }

        /// Callback is continuation for call site
        private async void RequestPromotedPiece(UnityAction<string> callback)
        {
            Show(_game.CurrentTurnColor);

            await UniTask.WaitWhile(() => _promotedPieceType == PieceType.None);

            Hide();

            string pieceLetter = Board.GetPieceLetter(_promotedPieceType);
            _promotedPieceType = PieceType.None;

            callback?.Invoke(pieceLetter);
        }

        private void Show(PieceColor pieceColor)
        {
            if (pieceColor == PieceColor.White)
            {
                whiteRoot.SetActive(true);
            }
            else
            {
                blackRoot.SetActive(true);
            }

            StartCoroutine(ShowContinue());
            return;

            IEnumerator ShowContinue()
            {
                float t = 0;
                while (t < 1)
                {
                    t += Time.deltaTime/appearTimeSec;
                    canvasGroup.alpha = Easing.OutCubic(t);
                    yield return null;
                }
            }
        }

        private void Hide()
        {
            StartCoroutine(HideContinue());
            return;

            IEnumerator HideContinue()
            {
                float t = 1;
                while (t > 0)
                {
                    t -= Time.deltaTime / appearTimeSec;
                    canvasGroup.alpha = Easing.InCubic(t);
                    yield return null;
                }

                whiteRoot.SetActive(false);
                blackRoot.SetActive(false);
            }
        }
    }
}