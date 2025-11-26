using Chess3D.Runtime.Core;
using Chess3D.Runtime.Menu.UI.ViewModels;
using UnityEngine;
using UnityEngine.Serialization;

public class NoesisLoader : MonoBehaviour
{
    [FormerlySerializedAs("nx")] public NoesisXaml nXaml;

    private void Start()
    {
        var nView = gameObject.GetComponent<NoesisView>();
        nView.Xaml = nXaml;
        nView.LoadXaml(false);
        var content = nView.Content;
        content.DataContext = ServiceLocator.Resolve<MainMenuViewModel>();
    }
}
