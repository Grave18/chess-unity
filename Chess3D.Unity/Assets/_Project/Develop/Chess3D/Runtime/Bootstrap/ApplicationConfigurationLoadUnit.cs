using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Bootstrap
{
    [Preserve]
    public class ApplicationConfigurationLoadUnit : ILoadUnit
    {
        private readonly ISettingsService _settingsService;

        public ApplicationConfigurationLoadUnit(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public UniTask Load()
        {
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
            QualitySettings.SetQualityLevel(_settingsService.S.GraphicsSettings.Quality);

            InitializeResolution();

            Screen.SetResolution(_settingsService.S.GraphicsSettings.Width,
                _settingsService.S.GraphicsSettings.Height,
                (FullScreenMode)_settingsService.S.GraphicsSettings.FullScreenMode);

            return UniTask.CompletedTask;
        }

        private void InitializeResolution()
        {
            if (_settingsService.S.GraphicsSettings.Width == 0 ||
                _settingsService.S.GraphicsSettings.Height == 0)
            {
                _settingsService.S.GraphicsSettings.Width = Screen.currentResolution.width;
                _settingsService.S.GraphicsSettings.Height = Screen.currentResolution.height;
                _settingsService.Save();
            }
        }
    }
}