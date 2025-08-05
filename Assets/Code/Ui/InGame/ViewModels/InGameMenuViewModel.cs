using System;
using ChessGame.Logic.MenuStates;
using MvvmTool;
using UnityEngine;

namespace Ui.InGame.ViewModels
{
    [INotifyPropertyChanged]
    public partial class InGameMenuViewModel : MonoBehaviour
    {
        [SerializeField] private PopupViewModel popupViewModel;
        [SerializeField] private MenuStateMachine menuStateMachine;

        [ObservableProperty] private bool _isOpened;

        public event Action<bool> OnOpenedChanged;

        // For Game Popup needs
        public PopupViewModel Popup => popupViewModel;

        [RelayCommand]
        private void OpenClose()
        {
            if (IsOpened)
            {
                menuStateMachine.ClosePause();
            }
            else
            {
                menuStateMachine.OpenPause();
            }

            OnOpenedChanged?.Invoke(IsOpened);
        }
    }
}