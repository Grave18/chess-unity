using MvvmTool;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    public partial class HomeViewModel : IViewModel
    {
        private readonly ViewNavigator _viewNavigator;

        public HomeViewModel(ViewNavigator viewNavigator)
        {
            _viewNavigator = viewNavigator;
        }

        [RelayCommand]
        private void Board()
        {
            _viewNavigator.GoTo<BoardViewModel>();
        }
    }
}