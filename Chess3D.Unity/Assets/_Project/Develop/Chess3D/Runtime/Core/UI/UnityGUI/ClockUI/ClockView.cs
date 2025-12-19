using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Sound;
using TMPro;
using UniRx;
using UnityEngine;

namespace Chess3D.Runtime.Core.UI.UnityGUI.ClockUI
{
    public sealed class ClockView : MonoBehaviour
    {
        [Header("Ui")]
        [SerializeField] private TMP_Text whiteText;
        [SerializeField] private TMP_Text blackText;

        [Header("Settings")]
        [SerializeField] private float updateIntervalSec = 0.2f;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color alertColor;

        private bool _isInitialized;

        private CoreEvents _coreEvents;
        private IClock _clock;
        private EffectsPlayer _effectsPlayer;

        public void Construct(CoreEvents coreEvents, IClock clock, EffectsPlayer effectsPlayer)
        {
            _clock = clock;
            _coreEvents = coreEvents;
            _effectsPlayer = effectsPlayer;

            _coreEvents.OnWarmup.Subscribe(_ => ResetColors()).AddTo(this);

            _isInitialized = true;
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
                _effectsPlayer.PlayTenSecondsSound();
            }
            else if (bSec > time - halfDt && bSec < time + halfDt)
            {
                blackText.color = alertColor;
                _effectsPlayer.PlayTenSecondsSound();
            }
        }
    }
}