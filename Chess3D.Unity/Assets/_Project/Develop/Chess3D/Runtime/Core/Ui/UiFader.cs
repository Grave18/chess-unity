using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Chess3D.Runtime.Core.Ui
{
    public class UiFader : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float speed = 1f;
        [SerializeField] private float delay = 1f;
        [SerializeField] private bool  isOnStart;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            Assert.IsNotNull(_canvasGroup);
        }

        private void Start()
        {
            if (isOnStart)
            {
                UnFade();
            }
        }

        public void Fade()
        {
            StartCoroutine(FadeRoutine(isFade: true));
        }

        public void UnFade()
        {
            StartCoroutine(FadeRoutine(isFade: false));
        }

        private IEnumerator FadeRoutine(bool isFade)
        {
            _canvasGroup.alpha = isFade ? 1f : 0f;
            yield return new WaitForSeconds(delay);

            if (isFade)
            {
                float t = 1f;
                while (t > 0f)
                {
                    t -= Time.deltaTime * speed;
                    _canvasGroup.alpha = t;

                    yield return null;
                }
            }
            else
            {
                float t = 0f;
                while (t < 1f)
                {
                    t += Time.deltaTime * speed;
                    _canvasGroup.alpha = t;

                    yield return null;
                }
            }
        }
    }
}