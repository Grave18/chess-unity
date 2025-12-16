using Chess3D.Runtime.Bootstrap.Settings;
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
        private readonly ISettingsContainer settingsContainer;

        public ApplicationConfigurationLoadUnit(ISettingsContainer settingsContainer)
        {
            this.settingsContainer = settingsContainer;
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
            QualitySettings.SetQualityLevel(settingsContainer.S.GraphicsSettings.Quality);
        }

        private void SetResolution()
        {
            InitializeResolution();
            Screen.SetResolution(settingsContainer.S.GraphicsSettings.Width,
                settingsContainer.S.GraphicsSettings.Height,
                (FullScreenMode)settingsContainer.S.GraphicsSettings.FullScreenMode);
        }

        private void InitializeResolution()
        {
            if (settingsContainer.S.GraphicsSettings.Width == 0 ||
                settingsContainer.S.GraphicsSettings.Height == 0)
            {
                settingsContainer.S.GraphicsSettings.Width = Screen.currentResolution.width;
                settingsContainer.S.GraphicsSettings.Height = Screen.currentResolution.height;
                settingsContainer.Save();
            }
        }

        private void SetVolume()
        {
            var audioMixer = AssetService.R.Load<AudioMixer>(RuntimeConstants.Settings.AudioMixerFileName);
            audioMixer.SetFloat(RuntimeConstants.Settings.MasterVolumeKey, VolumeUtil.Map01ToDb(settingsContainer.S.AudioSettings.MasterVolume));
            audioMixer.SetFloat(RuntimeConstants.Settings.MusicVolumeKey, VolumeUtil.Map01ToDb(settingsContainer.S.AudioSettings.MusicVolume));
            audioMixer.SetFloat(RuntimeConstants.Settings.EffectsVolumeKey, VolumeUtil.Map01ToDb(settingsContainer.S.AudioSettings.EffectsVolume));
        }
    }
}