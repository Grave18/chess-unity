using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameAndScene;
using Noesis;
using UnityEngine;

namespace Ui.Noesis
{
    public sealed class VsPlayerPageViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private SceneLoader sceneLoader;

        public DelegateCommand StartMatchCommand { get; private set; }

        public ObservableCollection<string> Items { get; private set; }

        private string _selectedItem;
        public string SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    SetField(ref _selectedItem, value);
                }
            }
        }

        private void Awake()
        {
            StartMatchCommand = new DelegateCommand(StartMatch);
            Items = new ObservableCollection<string> { "1", "3", "5", "10", "20", "30", "40", "50", "60" };

            SetTime();

            PropertyChanged += OnSelectedItemChanged;
        }

        private void StartMatch(object obj)
        {
            gameSettingsContainer.SetupGameOffline();
            sceneLoader.LoadGame();
        }

        private void SetTime()
        {
            string timeString = gameSettingsContainer.GetTimeString();
            SelectedItem = timeString;
        }

        private void OnSelectedItemChanged(object sender, PropertyChangedEventArgs e)
        {
            gameSettingsContainer.SetTime(SelectedItem);
            Log.Debug($"SelectedItem changed to {SelectedItem}");
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