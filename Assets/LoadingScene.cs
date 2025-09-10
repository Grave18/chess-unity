using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UtilsCommon.Singleton;

public class LoadingScene : SingletonBehaviour<LoadingScene>
{
    [SerializeField] private Image loadingBarImage;
    [SerializeField] private CanvasGroup faderImage;

    [Header("Settings")]
    [SerializeField] private float fadeTimeSec = 1f;
    [SerializeField] private float unFadeTimeSec = 1f;

    public float LoadingProgress
    {
        get => loadingBarImage.fillAmount;
        set => loadingBarImage.fillAmount = value;
    }

    public IEnumerator Fade()
    {
        while (faderImage.alpha < 1)
        {
            faderImage.alpha += Time.deltaTime * 1f/fadeTimeSec;
            yield return null;
        }
    }

    public IEnumerator UnFade()
    {
        while (faderImage.alpha > 0)
        {
            faderImage.alpha -= Time.deltaTime * 1f/unFadeTimeSec;
            yield return null;
        }
    }
}