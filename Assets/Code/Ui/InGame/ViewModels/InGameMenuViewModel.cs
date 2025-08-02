using System.Windows.Input;
using ChessGame.Logic.MenuStates;
using MvvmTool;
using Ui.Auxiliary;
using UnityEngine;

namespace Ui.InGame.ViewModels
{
    [INotifyPropertyChanged]
    public partial class InGameMenuViewModel : MonoBehaviour
    {
        private DelegateCommand _openCloseCommand;
        public ICommand OpenCloseCommand => _openCloseCommand ??= new DelegateCommand(OpenCloseLeftPanel);

        [ObservableProperty]
        private bool _isOpened;

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
    }
}