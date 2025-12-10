using Chess3D.Runtime.Core.UI.ViewModels;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using Noesis;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Core.UI
{
    [Preserve]
    public class CoreMainViewLoader : ILoadUnit
    {
        private readonly NoesisXaml _mainViewXaml;
        private readonly NoesisView _noesisView;
        private readonly CoreMainViewModel _mainViewModel;

        public CoreMainViewLoader(CoreMainViewModel mainViewModel, NoesisView noesisView, NoesisXaml mainViewXaml)
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
