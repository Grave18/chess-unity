using System;
using System.Collections;
using Chess3D.Runtime.ChessBoard;
using Chess3D.Runtime.ChessBoard.Info;
using Chess3D.Runtime.Logic;
using Chess3D.Runtime.UtilsCommon.Mathematics;
using UnityEngine;

namespace Chess3D.Runtime.UnityUi.InGame.Promotion
{
    public class PromotionPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject whiteRoot;
        [SerializeField] private GameObject blackRoot;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Settings")]
        [SerializeField] private float appearTimeSec = 0.2f;

        private PieceType _promotedPieceType;

        public void Select(PieceType pieceType)
        {
            _promotedPieceType = pieceType;
        }

        /// Callback is continuation for call site
        public void RequestPromotedPiece(PieceColor turnColor, Action<string> callback)
        {
            Show(turnColor);
            StartCoroutine(RequestPromotedPieceContinuation());
            return;

            IEnumerator RequestPromotedPieceContinuation()
            {
                yield return new WaitWhile(() => _promotedPieceType == PieceType.None);
                Hide();

                string pieceLetter = Board.GetPieceLetter(_promotedPieceType);
                _promotedPieceType = PieceType.None;

                callback?.Invoke(pieceLetter);
            }
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