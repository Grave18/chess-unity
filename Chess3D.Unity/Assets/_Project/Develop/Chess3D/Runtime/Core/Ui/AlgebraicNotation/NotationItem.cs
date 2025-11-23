using TMPro;
using UnityEngine;

namespace Chess3D.Runtime.Core.Ui.AlgebraicNotation
{
    public class NotationItem : MonoBehaviour
    {
        [Header("Ui")]
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private TMP_Text countText;
        [SerializeField] private TMP_Text whiteText;
        [SerializeField] private TMP_Text blackText;
        [SerializeField] private TMP_Text endGameText;

        [Header("Settings")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color fadeColor = Color.gray;

        public void AddEndGame(string endGameResult)
        {
            IncreaseHeight();
            endGameText.text = endGameResult;
            endGameText.color = normalColor;
        }

        private void IncreaseHeight()
        {
            Vector2 temp = rectTransform.sizeDelta;
            temp.y = 100f;
            rectTransform.sizeDelta = temp;
        }

        public void AddWhite(string algebraicNotation, int fullMoveCounter)
        {
            countText.text = fullMoveCounter == -1 ? string.Empty : fullMoveCounter.ToString();
            whiteText.text = algebraicNotation;
            whiteText.color = normalColor;
            RestoreHeight();
        }

        public void AddBlack(string algebraicNotation, int panelCount = -1)
        {
            if (panelCount != -1)
            {
                countText.text = panelCount.ToString();
                whiteText.text = "—";
            }

            blackText.text = algebraicNotation;
            blackText.color = normalColor;
            RestoreHeight();
        }

        private void RestoreHeight()
        {
            Vector2 temp = rectTransform.sizeDelta;
            temp.y = 50f;
            rectTransform.sizeDelta = temp;
            endGameText.text = string.Empty;
        }

        public void RemoveBlack()
        {
            blackText.text = string.Empty;
        }

        public void FadeWhite()
        {
            countText.color = fadeColor;
            whiteText.color = fadeColor;
            endGameText.color = fadeColor;
        }

        public void UnFadeWhite()
        {
            countText.color = normalColor;
            whiteText.color = normalColor;
        }

        public void FadeBlack()
        {
            blackText.color = fadeColor;
            endGameText.color = fadeColor;
        }

        public void UnFadeBlack()
        {
            blackText.color = normalColor;
        }
    }
}