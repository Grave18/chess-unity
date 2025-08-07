using System.Collections;
using Logic;
using Sound;
using TMPro;
using UnityEngine;

namespace Ui.Game
{
    public class ClockPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Initialization.Initialization initialization;
        [SerializeField] private Logic.Game game;

        [Header("Ui")]
        [SerializeField] private TMP_Text whiteText;
        [SerializeField] private TMP_Text blackText;

        [Header("Settings")]
        [SerializeField] private float updateIntervalSec = 0.2f;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color alertColor;

        private IClock _clock;

        public void Init(IClock clock)
        {
            _clock = clock;

            StartCoroutine(TickClock());
        }

        private IEnumerator TickClock()
        {
            var wait = new WaitForSeconds(updateIntervalSec);

            while (Application.isPlaying)
            {
                FormatAndApplyTime();
                PlayTenSecondsSoundAndChangeColor();

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

            private void FormatAndApplyTime()
            {
                int wMin = (int)_clock.WhiteTime.TotalMinutes;
                int wSec = _clock.WhiteTime.Seconds;
                int bMin = (int)_clock.BlackTime.TotalMinutes;
                int bSec = _clock.BlackTime.Seconds;

                whiteText.text = $"{wMin:00}:{wSec:00}";
                blackText.text = $"{bMin:00}:{bSec:00}";
            }

            private void PlayTenSecondsSoundAndChangeColor()
            {
                double wSec = _clock.WhiteTime.TotalSeconds;
                double bSec = _clock.BlackTime.TotalSeconds;
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