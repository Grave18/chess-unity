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

        private readonly ISettingsService _settingsService;

        public AudioSettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public UniTask Load()
        {
            _masterVolume = _settingsService.S.AudioSettings.MasterVolume;
            _musicVolume = _settingsService.S.AudioSettings.MusicVolume;
            _effectsVolume = _settingsService.S.AudioSettings.EffectsVolume;
            _audioMixer = AssetService.R.Load<AudioMixer>(RuntimeConstants.Settings.AudioMixerFileName);

            return UniTask.CompletedTask;
        }

        partial void OnMasterVolumeChanged(float value)
        {
            _settingsService.S.AudioSettings.MasterVolume = value;
            _audioMixer.SetFloat(RuntimeConstants.Settings.MasterVolumeKey, VolumeUtil.Map01ToDb(value));
            _settingsService.Save();
        }

        partial void OnMusicVolumeChanged(float value)
        {
            _settingsService.S.AudioSettings.MusicVolume = value;
            _audioMixer.SetFloat(RuntimeConstants.Settings.MusicVolumeKey, VolumeUtil.Map01ToDb(value));
            _settingsService.Save();
        }

        partial void OnEffectsVolumeChanged(float value)
        {
            _settingsService.S.AudioSettings.EffectsVolume = value;
            _audioMixer.SetFloat(RuntimeConstants.Settings.EffectsVolumeKey, VolumeUtil.Map01ToDb(value));
            _settingsService.Save();
        }
    }
}