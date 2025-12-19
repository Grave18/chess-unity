using Chess3D.Runtime.Core.MainCamera;
using DG.Tweening;
using TMPro;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.UI.UnityGUI.BoardUI
{
    public class NameView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text whitePlayerNameText;
        [SerializeField] private TMP_Text blackPlayerNameText;

        [Header("Settings")]
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private Ease fadeEasing = Ease.InOutCubic;

        private ISettingsContainer _settingsContainer;
        private Camera _uguiCamera;

        public Camera UGUICamera => _uguiCamera;
        public float FadeDuration => fadeDuration;
        public Ease FadeEasing => fadeEasing;

        [Inject]
        public void Construct(ISettingsContainer settingsContainer, [Key(CameraKeys.Ui)] Camera uguiCamera)
        {
            _settingsContainer = settingsContainer;
            _uguiCamera = uguiCamera;

            whitePlayerNameText.text = _settingsContainer.S.GameSettings.PlayerWhite.Name;
            blackPlayerNameText.text = _settingsContainer.S.GameSettings.PlayerBlack.Name;
        }
    }
}
