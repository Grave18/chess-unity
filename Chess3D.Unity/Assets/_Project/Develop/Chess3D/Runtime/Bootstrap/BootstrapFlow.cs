using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Chess3D.Runtime.Bootstrap
{
    public class BootstrapFlow : IStartable
    {
        private LoadingService _loadingService;
        private SceneManager _sceneManager;

        public BootstrapFlow(LoadingService loadingService, SceneManager sceneManager)
        {
            _loadingService = loadingService;
            _sceneManager = sceneManager;
        }

        public void Start()
        {
            _sceneManager.LoadScene(RuntimeConstants.Scenes.Menu).Forget();
        }
    }
}