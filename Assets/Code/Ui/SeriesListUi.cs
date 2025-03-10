using AlgebraicNotation;
using Logic;
using TMPro;
using UnityEngine;

namespace Ui
{
    public class SeriesListUi : MonoBehaviour
    {
        // [SerializeField] private GameManager gameManager;

        [SerializeField] private SeriesList seriesList;
        [SerializeField] private TMP_Text seriesText;

        private void Update()
        {
            seriesText.text = seriesList.GetSeriesText();
        }

        // private void OnEnable()
        // {
        //     gameManager.OnTurnChanged += UpdateText;
        // }
        //
        // private void OnDisable()
        // {
        //     gameManager.OnTurnChanged -= UpdateText;
        // }
        //
        // private void UpdateText(PieceColor pieceColor, CheckType checkType)
        // {
        //     seriesText.text = seriesList.GetSeriesText();
        // }
    }
}
