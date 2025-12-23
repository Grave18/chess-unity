using DG.Tweening;
using UnityEngine;

namespace Chess3D.Runtime.Core.Sound
{
    public sealed class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip mainTheme;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private Ease easing = Ease.InCubic;

        private void Start()
        {
            audioSource.clip = mainTheme;
            audioSource.loop = true;
            audioSource.Play();
            audioSource.DOFade(1f, fadeDuration)
                .SetEase(Ease.OutBounce)
                .From(0f)
                .SetEase(easing);
        }
    }
}
