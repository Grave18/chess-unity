using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using Noesis;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI
{
    [Preserve]
    public class MainViewLoader : ILoadUnit
    {
        private readonly NoesisXaml _mainViewXaml;
        private readonly NoesisView _noesisView;
        private readonly MainViewModel _mainViewModel;

        public MainViewLoader(MainViewModel mainViewModel, NoesisView noesisView, NoesisXaml mainViewXaml)
        {
            _mainViewModel = mainViewModel;
            _noesisView = noesisView;
            _mainViewXaml = mainViewXaml;
        }

        public UniTask Load()
        {
            _noesisView.Xaml = _mainViewXaml;
            _noesisView.LoadXaml(false);
            FrameworkElement mainView = _noesisView.Content;
            mainView.DataContext = _mainViewModel;

            return UniTask.CompletedTask;
        }
    }
}
