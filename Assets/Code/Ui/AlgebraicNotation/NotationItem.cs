using TMPro;
using UnityEngine;

namespace Ui.AlgebraicNotation
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

        public void AddWhite(string algebraicNotation, int panelCount)
        {
            countText.text = panelCount.ToString();
            whiteText.text = algebraicNotation;
            whiteText.color = normalColor;
        }

        public void AddBlack(string algebraicNotation)
        {
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