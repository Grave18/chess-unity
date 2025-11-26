using System.Collections.Generic;
using Chess3D.Runtime.Menu.UI.ViewModels;
using VContainer;

namespace Chess3D.Runtime.Menu.UI
{
    [UnityEngine.Scripting.Preserve]
    public class ViewNavigator
    {
        private readonly MainViewModel _mainViewModel;
        private readonly IObjectResolver _resolver;

        private readonly Stack<IViewModel> _viewModels = new();

        public ViewNavigator(MainViewModel mainViewModel, IObjectResolver resolver)
        {
            _mainViewModel = mainViewModel;
            _resolver = resolver;
        }

        public void GoTo<TViewModel>() where TViewModel : IViewModel
        {
            if (_mainViewModel.CurrentViewModel != null)
            {
                _viewModels.Push(_mainViewModel.CurrentViewModel);
            }

            _mainViewModel.CurrentViewModel = _resolver.Resolve<TViewModel>();
        }

        public void GoBack()
        {
            if (_viewModels.Count == 0)
            {
                return;
            }

            IViewModel viewModel = _viewModels.Pop();
            _mainViewModel.CurrentViewModel = viewModel;
        }
    }
}