using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Sound;
using TMPro;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.Ui.ClockUi
{
    public class ClockPanel : MonoBehaviour
    {
        private CoreEvents _coreEvents;

        [Header("Ui")]
        [SerializeField] private TMP_Text whiteText;
        [SerializeField] private TMP_Text blackText;

        [Header("Settings")]
        [SerializeField] private float updateIntervalSec = 0.2f;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color alertColor;

        private IClock _clock;

        private bool _isInitialized;

        public void Construct(CoreEvents coreEvents, IClock clock)
        {
            _clock = clock;
            _coreEvents = coreEvents;

            _coreEvents.OnWarmup += ResetColors;

            _isInitialized = true;
        }

        private void OnDestroy()
        {
            if(_coreEvents == null)
            {
                return;
            }

            _coreEvents.OnWarmup -= ResetColors;
        }

        private void Update()
        {
            if (!_isInitialized)
            {
                return;
            }

            FormatAndApplyTime();
            PlayTenSecondsSoundAndChangeColor();
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