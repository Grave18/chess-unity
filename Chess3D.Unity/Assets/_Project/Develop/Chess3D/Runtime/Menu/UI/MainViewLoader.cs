using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using Noesis;

namespace Chess3D.Runtime.Menu.UI
{
    [UnityEngine.Scripting.Preserve]
    public sealed class MainViewLoader : ILoadUnit
    {
        private readonly IMainViewModel _mainViewModel;
        private readonly NoesisXaml _mainViewXaml;
        private readonly NoesisView _noesisView;

        public MainViewLoader(NoesisView noesisView, NoesisXaml mainViewXaml, IMainViewModel mainViewModel)
        {
            _noesisView = noesisView;
            _mainViewXaml = mainViewXaml;
            _mainViewModel = mainViewModel;
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
