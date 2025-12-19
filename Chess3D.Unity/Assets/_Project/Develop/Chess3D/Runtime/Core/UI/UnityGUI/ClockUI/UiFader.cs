using DG.Tweening;
using UnityEngine;

namespace Chess3D.Runtime.Core.UI.UnityGUI.ClockUI
{
    public sealed class UiFader : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Settings")]
        [SerializeField] private float fadeDuration = 2f;
        [SerializeField] private float fadeDelay = 1f;
        [SerializeField] private Ease fadeEase = Ease.InOutCubic;
        [SerializeField] private bool isFadeOnStart;


        private void Start()
        {
            if (isFadeOnStart)
            {
                UnFade();
            }
        }

        public void Fade()
        {
            canvasGroup.DOFade(0f, fadeDuration)
                .From(1f)
                .SetDelay(fadeDelay)
                .SetEase(fadeEase);
        }

        public void UnFade()
        {
            canvasGroup.DOFade(1f, fadeDuration)
                .From(0f)
                .SetDelay(fadeDelay)
                .SetEase(fadeEase);
        }
    }
}