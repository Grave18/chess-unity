using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Chess3D.Runtime;
using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.Ai;
using Ui.Auxiliary;
using UnityEngine;

namespace Ui.Menu.ViewModels
{
    public class GameSettingsViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        // TODO: Add DI dependency
        private SettingsService _settingsService;

        public ObservableCollection<string> Times { get; set; }
        public ObservableCollection<string> Difficulties { get; set; }

        private string _playerName;
        public string PlayerName
        {
            get => _playerName;
            set
            {
                if (SetField(ref _playerName, value))
                {
                    // _settingsService.SetPlayerName(_playerName);
                    LogUi.Debug($"Name changed to {_playerName}");
                }
            }
        }

        private string _selectedTime;
        public string SelectedTime
        {
            get => _selectedTime;
            set
            {
                if (SetField(ref _selectedTime, value))
                {
                    // _settingsService.SetTime(_selectedTime);
                    LogUi.Debug($"Time changed to {_selectedTime}");
                }
            }
        }

        private string _selectedDifficulty;
        public string SelectedDifficulty
        {
            get => _selectedDifficulty;
            set
            {
                if (SetField(ref _selectedDifficulty, value))
                {
                    // _settingsService.SetDifficulty(_selectedDifficulty);
                    LogUi.Debug($"Difficulty changed to {_selectedDifficulty}");
                }
            }
        }

        private void Awake()
        {
            InitName();
            IntiTime();
            InitDifficulty();
        }

        private void InitName()
        {
            // _playerName = _settingsService.GetPlayerName();
        }

        private void IntiTime()
        {
            Times = new ObservableCollection<string> { "1", "3", "5", "10", "20", "30", "40", "50", "60" };
            // _selectedTime = _settingsService.GetTimeString();
        }

        private void InitDifficulty()
        {
            var difficulties = Enum.GetNames(typeof(ComputerSkillLevel));
            Difficulties = new ObservableCollection<string>(difficulties);
            // _selectedDifficulty = _settingsService.GetDifficulty();
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