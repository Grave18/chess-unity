using MvvmTool;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    public sealed partial class SettingsViewModel : IViewModel
    {
        private readonly ViewNavigator _viewNavigator;

        public SettingsViewModel(ViewNavigator viewNavigator)
        {
            _viewNavigator = viewNavigator;
        }

        [RelayCommand]
        private void Back()
        {
            _viewNavigator.GoBack();
        }
    }
}