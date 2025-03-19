using System.Collections;
using AlgebraicNotation;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class SeriesListPanel : MonoBehaviour
    {
        [SerializeField] private Game game;

        [SerializeField] private SeriesList seriesList;
        [SerializeField] private TMP_Text seriesText;
        [SerializeField] private ScrollRect scrollView;

        private void OnEnable()
        {
            // game.OnEndTurn += UpdateText;
        }

        private void OnDisable()
        {
            // game.OnEndTurn -= UpdateText;
        }

        private void UpdateText()
        {
            seriesText.text = seriesList.GetSeriesText();
            StartCoroutine(ResetScrollbar());
        }

        private IEnumerator ResetScrollbar()
        {
            yield return new WaitForEndOfFrame();
            scrollView.verticalNormalizedPosition = 0f;
        }
    }
}
