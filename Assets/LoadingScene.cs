using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UtilsCommon.Singleton;

public class LoadingScene : SingletonBehaviour<LoadingScene>
{
    [SerializeField] private Image loadingBarImage;
    [SerializeField] private RectTransform loadingPanel;
    [FormerlySerializedAs("faderImage")]
    [SerializeField] private CanvasGroup faderCanvasGroup;

    [Header("Settings")]
    [SerializeField] private float fadeTimeSec = 1f;
    [SerializeField] private Ease fadeEasing = Ease.OutBounce;
    [SerializeField] private float unfadeTimeSec = 1f;
    [SerializeField] private Ease unFadeEasing = Ease.OutBounce;


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
            .AsyncWaitForCompletion();
    }

    public async UniTask UnFade()
    {
        await loadingPanel
            .DOAnchorPos(new Vector2(1920, 0), unfadeTimeSec)
            .From(new Vector2(0, 0))
            .SetEase(fadeEasing, 2)
            .AsyncWaitForCompletion();
    }
}