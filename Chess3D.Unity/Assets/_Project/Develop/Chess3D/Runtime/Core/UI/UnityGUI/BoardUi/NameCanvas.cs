using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.UI.UnityGUI.BoardUi
{
    public class NameCanvas : MonoBehaviour, ILoadUnit
    {
        private ISettingsContainer settingsContainer;

        [SerializeField] private TMP_Text whitePlayerNameText;
        [SerializeField] private TMP_Text blackPlayerNameText;

        [Inject]
        public void Construct(ISettingsContainer settingsContainer)
        {
            this.settingsContainer = settingsContainer;
        }

        public UniTask Load()
        {
            whitePlayerNameText.text = settingsContainer.S.GameSettings.PlayerWhite.Name;
            blackPlayerNameText.text = settingsContainer.S.GameSettings.PlayerBlack.Name;

            return UniTask.CompletedTask;
        }
    }
}
