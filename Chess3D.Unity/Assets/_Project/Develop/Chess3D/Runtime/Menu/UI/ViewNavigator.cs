using System.Collections.Generic;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Utilities;
using Chess3D.Runtime.Utilities.Logging;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Chess3D.Runtime.Menu.UI
{
    [UnityEngine.Scripting.Preserve]
    public class ViewNavigator : ILoadUnit
    {
        private readonly IObjectResolver _resolver;

        private MainViewModel _mainViewModel;
        private readonly Stack<IViewModel> _viewModels = new();

        public ViewNavigator(IObjectResolver resolver)
        {
            _resolver = resolver;
        }


        public UniTask Load()
        {
            _mainViewModel = _resolver.Resolve<MainViewModel>();
            return UniTask.CompletedTask;
        }

        public void GoTo<TViewModel>() where TViewModel : IViewModel
        {
            if (_mainViewModel.CurrentViewModel is TViewModel)
            {
                Log.Noesis.D($"{typeof(TViewModel)} is already opened!");
                return;
            }

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