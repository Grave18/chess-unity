using Chess3D.Runtime.Utilities;
using System.Threading;
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

        public UniTask Load(CancellationToken cancellationToken = default)
        {
            _masterVolume = _settingsContainer.AudioSettings.MasterVolume;
            _musicVolume = _settingsContainer.AudioSettings.MusicVolume;
            _effectsVolume = _settingsContainer.AudioSettings.EffectsVolume;

            return UniTask.CompletedTask;
        }

        partial void OnMasterVolumeChanged(float value)
        {
            _settingsContainer.AudioSettings.MasterVolume = value;
            _audioMixer.SetFloat(RuntimeConstants.Settings.MasterVolumeKey, VolumeUtils.Map01ToDb(value));
            _settingsContainer.Save();
        }

        partial void OnMusicVolumeChanged(float value)
        {
            _settingsContainer.AudioSettings.MusicVolume = value;
            _audioMixer.SetFloat(RuntimeConstants.Settings.MusicVolumeKey, VolumeUtils.Map01ToDb(value));
            _settingsContainer.Save();
        }

        partial void OnEffectsVolumeChanged(float value)
        {
            _settingsContainer.AudioSettings.EffectsVolume = value;
            _audioMixer.SetFloat(RuntimeConstants.Settings.EffectsVolumeKey, VolumeUtils.Map01ToDb(value));
            _settingsContainer.Save();
        }
    }
}