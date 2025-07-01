using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameAndScene;
using Ui.Menu.Auxiliary;
using UnityEngine;

namespace Ui.Noesis
{
    public class PlayPageViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private SceneLoader sceneLoader;

        public DelegateCommand PlayOfflineCommand { get; private set; }
        public DelegateCommand PlayWithComputerCommand { get; private set; }
        public DelegateCommand PlayOnlineCommand { get; private set; }

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
            PlayOfflineCommand = new DelegateCommand(PlayOffline);
            PlayWithComputerCommand = new DelegateCommand(PlayWithComputer);
            PlayOnlineCommand = new DelegateCommand(PlayOnline);

            SetTime();

            PropertyChanged += OnPropertyChanged;
        }

        private void SetTime()
        {
            Items = new ObservableCollection<string> { "1", "3", "5", "10", "20", "30", "40", "50", "60" };
            string timeString = gameSettingsContainer.GetTimeString();
            SelectedItem = timeString;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            gameSettingsContainer.SetTime(SelectedItem);
            Log.Debug($"SelectedItem changed to {SelectedItem}");
        }

        private void PlayOffline(object obj)
        {
            gameSettingsContainer.SetupGameOffline();
            sceneLoader.LoadGame();
        }

        private void PlayWithComputer(object obj)
        {
            gameSettingsContainer.SetupGameWithComputer();
            sceneLoader.LoadGame();
        }

        private void PlayOnline(object obj)
        {
            Log.Debug("PlayOnline Clicked");
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