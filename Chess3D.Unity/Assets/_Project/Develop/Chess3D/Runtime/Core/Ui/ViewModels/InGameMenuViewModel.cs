using System;
using Chess3D.Runtime.Core.Logic.MenuStates;
using MvvmTool;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Core.Ui.ViewModels
{
    [INotifyPropertyChanged]
    [Preserve]
    public partial class InGameMenuViewModel
    {
        private readonly PopupViewModel _popupViewModel;
        private readonly MenuStateMachine _menuStateMachine;

        [ObservableProperty] private bool _isOpened;

        public InGameMenuViewModel(PopupViewModel popupViewModel, MenuStateMachine menuStateMachine)
        {
            _popupViewModel = popupViewModel;
            _menuStateMachine = menuStateMachine;
        }

        public event Action<bool> OnOpenedChanged;

        // For Game Popup needs
        public PopupViewModel Popup => _popupViewModel;

        [RelayCommand]
        private void OpenClose()
        {
            if (IsOpened)
            {
                _menuStateMachine.ClosePause();
            }
            else
            {
                _menuStateMachine.OpenPause();
            }

            OnOpenedChanged?.Invoke(IsOpened);
        }
    }
}