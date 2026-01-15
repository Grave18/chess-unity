using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Chess3D.Runtime.Bootstrap
{
    public sealed class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Image loadingBarImage;
        [SerializeField] private RectTransform loadingPanel;
        [SerializeField] private CanvasGroup faderCanvasGroup;

        [Header("Settings")]
        [SerializeField] private float fadeTimeSec = 0.7f;
        [SerializeField] private Ease fadeEasing = Ease.OutQuart;
        [SerializeField] private float unfadeTimeSec = 0.35f;
        [SerializeField] private Ease unfadeEasing = Ease.InQuart;


        public float LoadingProgress
        {
            get => loadingBarImage.fillAmount;
            set => loadingBarImage.fillAmount = value;
        }

        public async UniTask Fade()
        {
            await loadingPanel
                .DOAnchorPos(new Vector2(0, 0), fadeTimeSec)
                .From(new Vector2(-1920, 0))
                .SetEase(fadeEasing)
                .SetLink(gameObject)
                .Play()
                .AsyncWaitForCompletion();
        }

        public async UniTask Unfade()
        {
            await loadingPanel
                .DOAnchorPos(new Vector2(1920, 0), unfadeTimeSec)
                .From(new Vector2(0, 0))
                .SetEase(unfadeEasing)
                .SetLink(gameObject)
                .Play()
                .AsyncWaitForCompletion();
        }
    }
}