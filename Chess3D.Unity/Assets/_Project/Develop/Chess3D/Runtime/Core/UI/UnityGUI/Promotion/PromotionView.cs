using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.ChessBoard.Info;
using Chess3D.Runtime.Core.Logic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace Chess3D.Runtime.Core.UI.UnityGUI.Promotion
{
    public sealed class PromotionView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject whiteRoot;
        [SerializeField] private GameObject blackRoot;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Settings")]
        [SerializeField] private float appearTimeSec = 0.3f;
        [SerializeField] private Ease appearEasing = Ease.InCubic;

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

            canvasGroup.DOFade(1, appearTimeSec)
                .From(0)
                .SetEase(appearEasing)
                .SetLink(gameObject)
                .Play();
        }

        private void Hide()
        {
            whiteRoot.SetActive(false);
            blackRoot.SetActive(false);

            canvasGroup.DOFade(0, appearTimeSec)
                .From(0)
                .SetEase(appearEasing)
                .SetLink(gameObject)
                .Play();
        }
    }
}