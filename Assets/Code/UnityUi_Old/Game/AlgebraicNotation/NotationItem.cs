using TMPro;
using UnityEngine;

namespace Ui.Game.AlgebraicNotation
{
    public class NotationItem : MonoBehaviour
    {
        [Header("Ui")]
        [SerializeField] private TMP_Text countText;
        [SerializeField] private TMP_Text whiteText;
        [SerializeField] private TMP_Text blackText;

        [Header("Settings")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color fadeColor = Color.gray;

        public void AddEndGame(string endGameResult)
        {
            AddWhite(endGameResult, -1);
        }

        public void AddWhite(string algebraicNotation, int fullMoveCounter)
        {
            countText.text = fullMoveCounter == -1 ? string.Empty : fullMoveCounter.ToString();
            whiteText.text = algebraicNotation;
            whiteText.color = normalColor;
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
        }

        public void RemoveBlack()
        {
            blackText.text = string.Empty;
        }

        public void FadeWhite()
        {
            countText.color = fadeColor;
            whiteText.color = fadeColor;
        }

        public void UnFadeWhite()
        {
            countText.color = normalColor;
            whiteText.color = normalColor;
        }

        public void FadeBlack()
        {
            blackText.color = fadeColor;
        }

        public void UnFadeBlack()
        {
            blackText.color = normalColor;
        }
    }
}