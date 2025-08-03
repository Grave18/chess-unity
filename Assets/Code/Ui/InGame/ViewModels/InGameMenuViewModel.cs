using System.ComponentModel;
using ChessGame.Logic.MenuStates;
using MvvmTool;
using UnityEngine;

namespace Ui.InGame.ViewModels
{
    // [INotifyPropertyChanged]
    public partial class InGameMenuViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // [ObservableProperty]
        private bool _isOpened;
        public bool IsOpened
        {
            get => _isOpened;
            set => _isOpened = value;

        }

        [ObservableProperty]
        public int lol;

        [RelayCommand]
        private void OpenClose(object obj)
        {
            // if (IsOpened)
            // {
            //     MenuStateMachine.Instance.ClosePause();
            // }
            // else
            // {
            //     MenuStateMachine.Instance.OpenPause();
            // }
        }

    }
}