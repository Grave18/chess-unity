using ChessGame.Logic.MenuStates;
using MvvmTool;
using UnityEngine;

namespace Ui.InGame.ViewModels
{
    [INotifyPropertyChanged]
    public partial class InGameMenuViewModel : MonoBehaviour
    {
        [SerializeField] private PopupViewModel popupViewModel;

        [ObservableProperty] private bool _isOpened;

        // For Game Popup needs
        public PopupViewModel Popup => popupViewModel;

        [RelayCommand]
        private void OpenClose(object obj)
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
    }
}