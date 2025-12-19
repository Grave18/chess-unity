using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Chess3D.Runtime.Core.UI.UnityGUI.BoardUI
{
    public class TextToCameraRotator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private NameView mainView;

        [Header("UI")]
        [SerializeField] private RectTransform nameTextTransform;

        [Header("Settings")]
        [SerializeField] private bool isUpsideDown;

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
            else if (NeedToFlipBack())
            {
                FlipBack();
            }
        }

        private bool IsNeedToFlip()
        {
            Vector3 nameTextUp = transform.up;
            Vector3 camUp = mainView.UGUICamera.transform.up;

            var dot = Vector3.Dot(nameTextUp, camUp);
            bool isNeedToFlip = dot <= 0f && !isUpsideDown;

            return isNeedToFlip;
        }

        private void Flip()
        {
            Rotate(true);
        }

        private bool NeedToFlipBack()
        {
            Vector3 nameTextUp = transform.up;
            Vector3 camUp = mainView.UGUICamera.transform.up;

            var dot = Vector3.Dot(nameTextUp, camUp);
            bool isNeedToFlipBack = dot > 0f && isUpsideDown;

            return isNeedToFlipBack;
        }

        private void FlipBack()
        {
            Rotate(false);
        }

        private void Rotate(bool isBecomeUpsideDown)
        {
            if (_isRotating)
            {
                return;
            }

            _isRotating = true;

            DOTween.Sequence()
                .Append(text.DOFade(0f, mainView.FadeDuration).SetEase(mainView.FadeEasing))
                .AppendCallback(() =>
                {
                    float zAngle = isBecomeUpsideDown ? 180f : 0f;
                    nameTextTransform.localRotation = Quaternion.Euler(0f, 0f, zAngle);
                })
                .Append(text.DOFade(1f, mainView.FadeDuration).SetEase(mainView.FadeEasing))
                .AppendCallback(() =>
                {
                    isUpsideDown = isBecomeUpsideDown;
                    _isRotating = false;
                })
                .Play();
        }

    }
}