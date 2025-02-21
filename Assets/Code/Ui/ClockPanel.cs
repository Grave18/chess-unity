using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class ClockPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Clock clock;

        [Header("Ui")]
        [SerializeField] private TMP_Text whiteText;
        [SerializeField] private TMP_Text blackText;
        [SerializeField] private Button playPauseButton;

        private void OnEnable()
        {
            playPauseButton.onClick.AddListener(PlayPause);
        }

        private void PlayPause()
        {
            if (clock.IsPlaying)
            {
                clock.Pause();
            }
            else
            {
                clock.Play();
            }
        }

        private void Update()
        {
            whiteText.text = clock.WhiteTime.ToString(@"mm\:ss");
            blackText.text = clock.BlackTime.ToString(@"mm\:ss");
        }
    }
}