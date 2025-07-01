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
        [SerializeField] private ChessGame.Logic.Game game;

        [Header("Ui")]
        [SerializeField] private TMP_Text whiteText;
        [SerializeField] private TMP_Text blackText;

        [Header("Settings")]
        [SerializeField] private float updateIntervalSec = 0.2f;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color alertColor;

        private IEnumerator Start()
        {
            var wait = new WaitForSeconds(updateIntervalSec);
            IClock clock = initialization.Clock;

            yield return WaitForClockInit(clock);

            while (Application.isPlaying)
            {
                FormatAndApplyTime(clock);
                PlayTenSecondsSoundAndChangeColor(clock);

                yield return wait;
            }
        }

        private void OnEnable()
        {
            game.OnWarmup += ResetColors;
        }

        private void OnDisable()
        {
            game.OnWarmup -= ResetColors;
        }

        private void ResetColors()
        {
            whiteText.color = normalColor;
            blackText.color = normalColor;
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

        private  void PlayTenSecondsSoundAndChangeColor(IClock clock)
        {
            double wSec = clock.WhiteTime.TotalSeconds;
            double bSec = clock.BlackTime.TotalSeconds;
            const float time = 10f;
            float halfDt = updateIntervalSec * 0.525f;

            if (wSec > time - halfDt && wSec < time + halfDt)
            {
                whiteText.color = alertColor;
                EffectsPlayer.Instance.PlayTenSecondsSound();
            }
            else if (bSec > time - halfDt && bSec < time + halfDt)
            {
                blackText.color = alertColor;
                EffectsPlayer.Instance.PlayTenSecondsSound();
            }
        }
    }
}