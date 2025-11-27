using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using Noesis;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime
{
    public class MainViewLoader : MonoBehaviour, ILoadUnit
    {
        [SerializeField] public NoesisXaml mainViewXaml;

        private MainViewModel _mainViewModel;

        [Inject]
        public void Construct(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public UniTask Load()
        {
            var noesisView = gameObject.GetComponent<NoesisView>();
            noesisView.Xaml = mainViewXaml;
            noesisView.LoadXaml(false);
            FrameworkElement content = noesisView.Content;
            content.DataContext = _mainViewModel;

            return UniTask.CompletedTask;
        }
    }
}
