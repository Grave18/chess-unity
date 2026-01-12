using DG.Tweening;
using UnityEngine;

namespace Chess3D.Runtime.Menu.Sound
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
                .From(0f)
                .SetEase(easing)
                .SetLink(gameObject)
                .Play();
        }
    }
}
