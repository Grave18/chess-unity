using System.Collections;
using ChessGame.Logic;
using TMPro;
using UnityEngine;

namespace Ui.Game
{
    public class ClockPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Initialization.Initialization initialization;

        [Header("Ui")]
        [SerializeField] private TMP_Text whiteText;
        [SerializeField] private TMP_Text blackText;

        private IEnumerator Start()
        {
            var wait = new WaitForSeconds(0.2f);
            IClock clock = initialization.Clock;

            yield return WaitForClockInit(clock);

            while (Application.isPlaying)
            {
                FormatAndApplyTime(clock);

                yield return wait;
            }
        }

        private static WaitWhile WaitForClockInit(IClock clock)
        {
            return new WaitWhile (() => clock == null);
        }

        private void FormatAndApplyTime(IClock clock)
        {
            int wMin = (int)clock.WhiteTime.TotalMinutes;
            int wSec = clock.WhiteTime.Seconds;
            int bMin = (int)clock.BlackTime.TotalMinutes;
            int bSec = clock.BlackTime.Seconds;

            whiteText.text = $"{wMin:00}:{wSec:00}";
            blackText.text = $"{bMin:00}:{bSec:00}";
        }
    }
}