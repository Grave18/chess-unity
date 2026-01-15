using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Bootstrap
{
    [Preserve]
    public sealed class ApplicationConfigurationLoadUnit : ILoadUnit
    {
        private readonly ISettingsContainer settingsContainer;
        private readonly AudioMixer _audioMixer;

        public ApplicationConfigurationLoadUnit(ISettingsContainer settingsContainer, AudioMixer audioMixer)
        {
            this.settingsContainer = settingsContainer;
            _audioMixer = audioMixer;
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
            QualitySettings.SetQualityLevel(settingsContainer.GraphicsSettings.Quality);
        }

        private void SetResolution()
        {
            InitializeResolution();
            Screen.SetResolution(settingsContainer.GraphicsSettings.Width,
                settingsContainer.GraphicsSettings.Height,
                (FullScreenMode)settingsContainer.GraphicsSettings.FullScreenMode);
        }

        private void InitializeResolution()
        {
            if (settingsContainer.GraphicsSettings.Width == 0 ||
                settingsContainer.GraphicsSettings.Height == 0)
            {
                settingsContainer.GraphicsSettings.Width = Screen.currentResolution.width;
                settingsContainer.GraphicsSettings.Height = Screen.currentResolution.height;
                settingsContainer.Save();
            }
        }

        private void SetVolume()
        {
            _audioMixer.SetFloat(RuntimeConstants.Settings.MasterVolumeKey, VolumeUtils.Map01ToDb(settingsContainer.AudioSettings.MasterVolume));
            _audioMixer.SetFloat(RuntimeConstants.Settings.MusicVolumeKey, VolumeUtils.Map01ToDb(settingsContainer.AudioSettings.MusicVolume));
            _audioMixer.SetFloat(RuntimeConstants.Settings.EffectsVolumeKey, VolumeUtils.Map01ToDb(settingsContainer.AudioSettings.EffectsVolume));
        }
    }
}