using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.Notation;
using Ui.Auxiliary;
using MvvmTool;
using UnityEngine;

namespace Ui.Menu.ViewModels
{
    public class FenUserControlViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        public DelegateCommand GetDefaultFenCommand { get; private set; }
        public DelegateCommand GetSavedFenCommand { get; private set; }

        private string _currentFen;
        public string CurrentFen
        {
            get => _currentFen;
            set
            {
                if (FenValidator.IsValid(value, out string message) && SetField(ref _currentFen, value))
                {
                    gameSettingsContainer.SetCurrentFen(_currentFen);
                    LogUi.Debug($"Current Fen changed to {_currentFen}");
                }
                else if(message is not {Length: 0})
                {
                    LogUi.Debug(message);
                }
            }
        }

        private void Awake()
        {
            GetSavedFenCommand = new DelegateCommand(GetSavedFen);
            GetDefaultFenCommand = new DelegateCommand(GetDefaultFen);

            InitFen();
        }

        private void GetDefaultFen(object obj)
        {
            CurrentFen = gameSettingsContainer.GetDefaultFen();
        }

        private void GetSavedFen(object obj)
        {
            CurrentFen = gameSettingsContainer.GetSavedFen();
        }

        private void InitFen()
        {
            _currentFen = gameSettingsContainer.GetCurrentFen();
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