using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
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
            SetFramerate();
            SetQuality();
            SetResolution();
            SetVolume();

            return UniTask.CompletedTask;
        }

        private static void SetFramerate()
        {
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
        }

        private void SetQuality()
        {
            QualitySettings.SetQualityLevel(_settingsService.S.GraphicsSettings.Quality);
        }

        private void SetResolution()
        {
            InitializeResolution();
            Screen.SetResolution(_settingsService.S.GraphicsSettings.Width,
                _settingsService.S.GraphicsSettings.Height,
                (FullScreenMode)_settingsService.S.GraphicsSettings.FullScreenMode);
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

        private void SetVolume()
        {
            var audioMixer = AssetService.R.Load<AudioMixer>(RuntimeConstants.Settings.AudioMixerFileName);
            audioMixer.SetFloat(RuntimeConstants.Settings.MasterVolumeKey, VolumeUtil.Map01ToDb(_settingsService.S.AudioSettings.MasterVolume));
            audioMixer.SetFloat(RuntimeConstants.Settings.MusicVolumeKey, VolumeUtil.Map01ToDb(_settingsService.S.AudioSettings.MusicVolume));
            audioMixer.SetFloat(RuntimeConstants.Settings.EffectsVolumeKey, VolumeUtil.Map01ToDb(_settingsService.S.AudioSettings.EffectsVolume));
        }
    }
}