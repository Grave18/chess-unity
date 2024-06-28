using Logic;
using Logic.Notation;
using TMPro;
using UnityEngine;

namespace Ui
{
    public class SeriesListUi : MonoBehaviour
    {
        [SerializeField] private SeriesList seriesList;
        [SerializeField] private TMP_Text seriesText;

        private void Update()
        {
            seriesText.text = seriesList.GetSeriesText();
        }
    }
}
