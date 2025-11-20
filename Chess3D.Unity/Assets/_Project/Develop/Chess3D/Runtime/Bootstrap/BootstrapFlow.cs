using System.Threading;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using VContainer.Unity;

namespace Chess3D.Runtime.Bootstrap
{
    [Preserve]
    public class BootstrapFlow : IAsyncStartable
    {
        private readonly LoadingService _loadingService;
        private readonly SceneManager _sceneManager;
        private readonly SettingsService _settingsService;
        private readonly ApplicationConfigurationLoadUnit _applicationConfigurationLoadUnit;

        public BootstrapFlow(LoadingService loadingService, SceneManager sceneManager, SettingsService settingsService,
            ApplicationConfigurationLoadUnit applicationConfigurationLoadUnit)
        {
            _loadingService = loadingService;
            _sceneManager = sceneManager;
            _settingsService = settingsService;
            _applicationConfigurationLoadUnit = applicationConfigurationLoadUnit;
        }

        public async UniTask StartAsync(CancellationToken cancellation = new())
        {
            await _loadingService.BeginLoading(_settingsService);
            await _loadingService.BeginLoading(_applicationConfigurationLoadUnit);

            await _sceneManager.LoadScene(RuntimeConstants.Scenes.Core);
        }
    }
}