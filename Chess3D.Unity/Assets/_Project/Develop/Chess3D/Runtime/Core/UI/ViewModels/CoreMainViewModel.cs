using Chess3D.Runtime.Core.Logic.MenuStates;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using MvvmTool;
using UniRx;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Core.UI.ViewModels
{
    [Preserve]
    [INotifyPropertyChanged]
    public partial class CoreMainViewModel : IMainViewModel, ILoadUnit<IViewModel>
    {
        [ObservableProperty] private IViewModel _currentViewModel;
        [ObservableProperty] private bool _isOpened;

        public readonly Subject<bool> OnOpenedChanged = new();

        private readonly MenuStateMachine _menuStateMachine;

        public CoreMainViewModel(MenuStateMachine menuStateMachine)
        {
            _menuStateMachine = menuStateMachine;
        }

        public UniTask Load(IViewModel viewModel)
        {
            CurrentViewModel = viewModel;

            return UniTask.CompletedTask;
        }

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

            OnOpenedChanged.OnNext(IsOpened);
        }
    }
}