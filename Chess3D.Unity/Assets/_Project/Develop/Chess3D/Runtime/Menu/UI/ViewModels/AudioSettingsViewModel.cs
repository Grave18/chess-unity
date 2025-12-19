using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using MvvmTool;
using UnityEngine.Audio;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    [INotifyPropertyChanged]
    public sealed partial class AudioSettingsViewModel : ILoadUnit
    {
        [ObservableProperty] private float _masterVolume;
        [ObservableProperty] private float _musicVolume;
        [ObservableProperty] private float _effectsVolume;

        private readonly ISettingsContainer _settingsContainer;
        private readonly AudioMixer _audioMixer;

        public AudioSettingsViewModel(ISettingsContainer settingsContainer, AudioMixer audioMixer)
        {
            _settingsContainer = settingsContainer;
            _audioMixer = audioMixer;
        }

        public UniTask Load()
        {
            _masterVolume = _settingsContainer.S.AudioSettings.MasterVolume;
            _musicVolume = _settingsContainer.S.AudioSettings.MusicVolume;
            _effectsVolume = _settingsContainer.S.AudioSettings.EffectsVolume;

            return UniTask.CompletedTask;
        }

        partial void OnMasterVolumeChanged(float value)
        {
            _settingsContainer.S.AudioSettings.MasterVolume = value;
            _audioMixer.SetFloat(RuntimeConstants.Settings.MasterVolumeKey, VolumeUtils.Map01ToDb(value));
            _settingsContainer.Save();
        }

        partial void OnMusicVolumeChanged(float value)
        {
            _settingsContainer.S.AudioSettings.MusicVolume = value;
            _audioMixer.SetFloat(RuntimeConstants.Settings.MusicVolumeKey, VolumeUtils.Map01ToDb(value));
            _settingsContainer.Save();
        }

        partial void OnEffectsVolumeChanged(float value)
        {
            _settingsContainer.S.AudioSettings.EffectsVolume = value;
            _audioMixer.SetFloat(RuntimeConstants.Settings.EffectsVolumeKey, VolumeUtils.Map01ToDb(value));
            _settingsContainer.Save();
        }
    }
}