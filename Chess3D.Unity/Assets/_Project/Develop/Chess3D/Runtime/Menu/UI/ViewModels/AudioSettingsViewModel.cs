using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using MvvmTool;
using UnityEngine.Audio;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    [INotifyPropertyChanged]
    public partial class AudioSettingsViewModel : ILoadUnit
    {
        [ObservableProperty] private float _masterVolume;
        [ObservableProperty] private float _musicVolume;
        [ObservableProperty] private float _effectsVolume;

        private AudioMixer _audioMixer;

        private readonly ISettingsContainer settingsContainer;

        public AudioSettingsViewModel(ISettingsContainer settingsContainer)
        {
            this.settingsContainer = settingsContainer;
        }

        public UniTask Load()
        {
            _masterVolume = settingsContainer.S.AudioSettings.MasterVolume;
            _musicVolume = settingsContainer.S.AudioSettings.MusicVolume;
            _effectsVolume = settingsContainer.S.AudioSettings.EffectsVolume;
            _audioMixer = AssetService.R.Load<AudioMixer>(RuntimeConstants.Settings.AudioMixerFileName);

            return UniTask.CompletedTask;
        }

        partial void OnMasterVolumeChanged(float value)
        {
            settingsContainer.S.AudioSettings.MasterVolume = value;
            _audioMixer.SetFloat(RuntimeConstants.Settings.MasterVolumeKey, VolumeUtil.Map01ToDb(value));
            settingsContainer.Save();
        }

        partial void OnMusicVolumeChanged(float value)
        {
            settingsContainer.S.AudioSettings.MusicVolume = value;
            _audioMixer.SetFloat(RuntimeConstants.Settings.MusicVolumeKey, VolumeUtil.Map01ToDb(value));
            settingsContainer.Save();
        }

        partial void OnEffectsVolumeChanged(float value)
        {
            settingsContainer.S.AudioSettings.EffectsVolume = value;
            _audioMixer.SetFloat(RuntimeConstants.Settings.EffectsVolumeKey, VolumeUtil.Map01ToDb(value));
            settingsContainer.Save();
        }
    }
}