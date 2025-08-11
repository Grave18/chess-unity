using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UtilsCommon;

namespace UnityUi.InGame.BoardUi
{
    public class TextToCameraRotator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera cam;
        [SerializeField] private RectTransform nameTextTransform;

        [Header("Settings")]
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private Ease easing = Ease.Linear;

        private bool _isUpsideDown;
        private bool _isRotating;
        private TMP_Text text;

        private void Awake()
        {
            text = nameTextTransform.GetComponent<TMP_Text>();
            Assert.IsNotNull(text);
        }

        private void Update()
        {
            if (IsNeedToFlip())
            {
                Flip();
            }

            if (NeedToFlipBack())
            {
                FlipBack();
            }
        }

        private bool IsNeedToFlip()
        {
            Vector3 nameTextUp = transform.up;
            Vector3 camUp = cam.transform.up;

            var dot = Vector3.Dot(nameTextUp, camUp);
            bool isNeedToFlip = dot <= 0f && !_isUpsideDown;

            return isNeedToFlip;
        }

        private void Flip()
        {
            CoroutineRunner.Run(FlipRoutine());
            return;

            IEnumerator FlipRoutine()
            {
                if (_isRotating)
                {
                    yield break;
                }

                _isRotating = true;
                text.DOFade(0f, duration).SetEase(easing);
                yield return new WaitForSeconds(duration);
                nameTextTransform.localRotation = Quaternion.Euler(0f, 0f, 180f);
                text.DOFade(1f, duration).SetEase(easing);
                yield return new WaitForSeconds(duration);
                _isUpsideDown = true;
                _isRotating = false;
            }
        }

        private bool NeedToFlipBack()
        {
            Vector3 nameTextUp = transform.up;
            Vector3 camUp = cam.transform.up;

            var dot = Vector3.Dot(nameTextUp, camUp);
            bool isNeedToFlipBack = dot > 0f && _isUpsideDown;

            return isNeedToFlipBack;
        }

        private void FlipBack()
        {
            CoroutineRunner.Run(FlipBackRoutine());
            return;

            IEnumerator FlipBackRoutine()
            {
                if (_isRotating)
                {
                    yield break;
                }

                _isRotating = true;
                text.DOFade(0f, duration).SetEase(easing);
                yield return new WaitForSeconds(duration);
                nameTextTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                text.DOFade(1f, duration).SetEase(easing);
                yield return new WaitForSeconds(duration);
                _isUpsideDown = false;
                _isRotating = false;
            }
        }
    }
}