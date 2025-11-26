using Chess3D.Runtime.Core;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Noesis;
using UnityEngine;
using UnityEngine.Scripting;

namespace Chess3D.Runtime
{
    [Preserve]
    public class NoesisLoader : MonoBehaviour
    {
        [SerializeField] public NoesisXaml mainViewXaml;

        private void Start()
        {
            var noesisView = gameObject.GetComponent<NoesisView>();
            noesisView.Xaml = mainViewXaml;
            noesisView.LoadXaml(false);
            FrameworkElement content = noesisView.Content;
            content.DataContext = ServiceLocator.Resolve<MainViewModel>();
        }
    }
}
