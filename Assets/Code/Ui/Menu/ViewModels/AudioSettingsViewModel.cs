using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameAndScene;
using Ui.Menu.Auxiliary;
using UnityEngine;

namespace Ui.Menu.ViewModels
{
    public class AudioSettingsViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        [SerializeField] private AudioSettingsContainer audioSettingsContainer;

        private float _masterVolume;

        public float MasterVolume
        {
            get => _masterVolume;
            set
            {
                if (SetField(ref _masterVolume, value))
                {
                    audioSettingsContainer.SetMasterVolume(value);
                    LogUi.Debug($"{nameof(MasterVolume)} changed to {value}");
                }
            }
        }

        private float _musicVolume;
        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                if (SetField(ref _musicVolume, value))
                {
                    audioSettingsContainer.SetMusicVolume(value);
                    LogUi.Debug($"{nameof(MusicVolume)} changed to {value}");
                }
            }
        }

        private float _effectsVolume;
        public float EffectsVolume
        {
            get => _effectsVolume;
            set
            {
                if (SetField(ref _effectsVolume, value))
                {
                    audioSettingsContainer.SetEffectsVolume(value);
                    LogUi.Debug($"{nameof(EffectsVolume)} changed to {value}");
                }
            }
        }

        private void Awake()
        {
            MasterVolume = audioSettingsContainer.GetMasterVolume();
            MusicVolume = audioSettingsContainer.GetMusicVolume();
            EffectsVolume = audioSettingsContainer.GetEffectsVolume();
        }

        #region ViewModelImplimentation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

#endregion Implimentation
    }
}