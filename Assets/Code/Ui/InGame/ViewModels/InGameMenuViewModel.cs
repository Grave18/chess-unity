using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChessGame.Logic.MenuStates;
using MvvmTool;
using Ui.Auxiliary;
using UnityEngine;

namespace Ui.InGame.ViewModels
{
    public partial class InGameMenuViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        public DelegateCommand OpenCloseCommand { get; set; }

        [ObservableProperty]
        private bool _isOpened;

        private void Awake()
        {
            OpenCloseCommand = new DelegateCommand(OpenCloseLeftPanel);
        }

        private void OpenCloseLeftPanel(object obj)
        {
            if (IsOpened)
            {
                MenuStateMachine.Instance.ClosePause();
            }
            else
            {
                MenuStateMachine.Instance.OpenPause();
            }
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