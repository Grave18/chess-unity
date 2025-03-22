using System;
using System.Collections;
using ChessBoard;
using ChessBoard.Info;
using Logic;
using UnityEngine;

namespace Ui.Promotion
{
    public class PromotionPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject whiteRoot;
        [SerializeField] private GameObject blackRoot;

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
        }

        private void Hide()
        {
            whiteRoot.SetActive(false);
            blackRoot.SetActive(false);
        }
    }
}