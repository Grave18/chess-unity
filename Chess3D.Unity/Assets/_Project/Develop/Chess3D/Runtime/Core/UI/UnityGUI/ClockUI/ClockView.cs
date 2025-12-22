using System;
using Chess3D.Runtime.Core.Logic;
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

        public void Construct(CoreEvents coreEvents, IClock clock)
        {
            coreEvents.OnWarmup.Subscribe(_ => ResetColors()).AddTo(this);
            clock.WhiteTime.Subscribe(time => FormatAndApplyTime(whiteText, time)).AddTo(this);
            clock.BlackTime.Subscribe(time => FormatAndApplyTime(blackText, time)).AddTo(this);
            clock.WhiteTenSeconds.Subscribe(_ => whiteText.color = alertColor).AddTo(this);
            clock.BlackTenSeconds.Subscribe(_ => blackText.color = alertColor).AddTo(this);
        }

        private void ResetColors()
        {
            whiteText.color = normalColor;
            blackText.color = normalColor;
        }

        private void FormatAndApplyTime(TMP_Text text, TimeSpan time)
        {
            int min = (int)time.TotalMinutes;
            int sec = time.Seconds;
            text.text = $"{min:00}:{sec:00}";
        }
    }
}