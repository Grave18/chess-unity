using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameAndScene;
using Ui.Menu.Auxiliary;
using UnityEngine;

namespace Ui.Menu.ViewModels
{
    public class GameSettingsViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        public ObservableCollection<string> Times { get; set; }
        public ObservableCollection<string> Difficulties { get; set; }

        private string _selectedTime;
        public string SelectedTime
        {
            get => _selectedTime;
            set
            {
                if (SetField(ref _selectedTime, value))
                {
                    gameSettingsContainer.SetTime(_selectedTime);
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
                    gameSettingsContainer.SetDifficulty(_selectedDifficulty);
                    LogUi.Debug($"Difficulty changed to {_selectedDifficulty}");
                }
            }
        }

        private void Awake()
        {
            SetTime();
            SetDifficulty();
        }

        private void SetTime()
        {
            Times = new ObservableCollection<string> { "1", "3", "5", "10", "20", "30", "40", "50", "60" };
            SelectedTime = gameSettingsContainer.GetTimeString();
        }

        private void SetDifficulty()
        {
            Difficulties = new ObservableCollection<string> { "Easy", "Medium", "Hard" };
            SelectedDifficulty = gameSettingsContainer.GetDifficulty();
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