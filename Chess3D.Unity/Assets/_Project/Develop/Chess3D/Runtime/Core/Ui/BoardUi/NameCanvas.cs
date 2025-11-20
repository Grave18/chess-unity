using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.Ui.BoardUi
{
    public class NameCanvas : MonoBehaviour, ILoadUnit
    {
        private SettingsService _settingsService;

        [SerializeField] private TMP_Text whitePlayerNameText;
        [SerializeField] private TMP_Text blackPlayerNameText;

        [Inject]
        public void Construct(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public UniTask Load()
        {
            whitePlayerNameText.text = _settingsService.S.GameSettings.PlayerWhite.Name;
            blackPlayerNameText.text = _settingsService.S.GameSettings.PlayerBlack.Name;

            return UniTask.CompletedTask;
        }
    }
}
