using TMPro;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.UI.UnityGUI.BoardUI
{
    public class NameView : MonoBehaviour
    {
        private ISettingsContainer _settingsContainer;

        [SerializeField] private TMP_Text whitePlayerNameText;
        [SerializeField] private TMP_Text blackPlayerNameText;

        [Inject]
        public void Construct(ISettingsContainer settingsContainer)
        {
            _settingsContainer = settingsContainer;
            whitePlayerNameText.text = _settingsContainer.S.GameSettings.PlayerWhite.Name;
            blackPlayerNameText.text = _settingsContainer.S.GameSettings.PlayerBlack.Name;
        }
    }
}
