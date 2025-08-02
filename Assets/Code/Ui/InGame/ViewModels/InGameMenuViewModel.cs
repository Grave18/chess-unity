using ChessGame.Logic.MenuStates;
using MvvmTool;
using UnityEngine;

namespace Ui.InGame.ViewModels
{
    [INotifyPropertyChanged]
    public partial class InGameMenuViewModel : MonoBehaviour
    {
        [ObservableProperty]
        private bool _isOpened;

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