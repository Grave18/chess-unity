using MvvmTool;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    [INotifyPropertyChanged]
    public sealed partial class HomeViewModel : IViewModel
    {
        private readonly ViewNavigator _viewNavigator;
        [ObservableProperty] private bool _isPopupOpen;

        public HomeViewModel(ViewNavigator viewNavigator)
        {
            _viewNavigator = viewNavigator;
        }

        [RelayCommand]
        private void Play()
        {
            _viewNavigator.GoTo<PlayViewModel>();
        }

        [RelayCommand]
        private void Settings()
        {
            _viewNavigator.GoTo<SettingsViewModel>();
        }

        [RelayCommand]
        private void Board()
        {
            _viewNavigator.GoTo<BoardViewModel>();
        }

        [RelayCommand]
        private void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}