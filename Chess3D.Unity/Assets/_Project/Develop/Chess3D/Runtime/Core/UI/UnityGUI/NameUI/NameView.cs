using System;
using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.MainCamera;
using DG.Tweening;
using TMPro;
using UnityEngine;
using VContainer;
using UniRx;

namespace Chess3D.Runtime.Core.UI.UnityGUI.NameUI
{
    public sealed class NameView : MonoBehaviour, IDisposable
    {
        [Header("UI")]
        [SerializeField] private TMP_Text whitePlayerNameText;
        [SerializeField] private TMP_Text blackPlayerNameText;

        [SerializeField] private TextToCameraRotator textToCameraRotatorWhite;
        [SerializeField] private TextToCameraRotator textToCameraRotatorBlack;

        [Header("Settings")]
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private Ease fadeEasing = Ease.InOutCubic;

        private readonly CompositeDisposable _disposables = new();

        [Inject]
        public void Construct([Key(CameraKeys.Ui)] Camera uguiCamera, PlayerNames playerNames)
        {
            textToCameraRotatorWhite.Construct(uguiCamera, fadeDuration, fadeEasing);
            textToCameraRotatorBlack.Construct(uguiCamera, fadeDuration, fadeEasing);

            playerNames.WhitePlayerName.Subscribe(whiteName =>
                whitePlayerNameText.text = whiteName)
                .AddTo(_disposables);

            playerNames.BlackPlayerName.Subscribe(blackName =>
                blackPlayerNameText.text = blackName)
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}