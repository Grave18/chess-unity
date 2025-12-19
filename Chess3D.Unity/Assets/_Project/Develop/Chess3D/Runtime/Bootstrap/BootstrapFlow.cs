using System.Threading;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using VContainer.Unity;

namespace Chess3D.Runtime.Bootstrap
{
    [Preserve]
    public sealed class BootstrapFlow : IAsyncStartable
    {
        private readonly LoadingService _loadingService;
        private readonly SceneManager _sceneManager;
        private readonly ISettingsContainer _settingsContainer;
        private readonly ConfigContainer _configContainer;
        private readonly ApplicationConfigurationLoadUnit _applicationConfigurationLoadUnit;

        public BootstrapFlow(LoadingService loadingService, SceneManager sceneManager, ISettingsContainer settingsContainer,
            ApplicationConfigurationLoadUnit applicationConfigurationLoadUnit, ConfigContainer configContainer)
        {
            _loadingService = loadingService;
            _sceneManager = sceneManager;
            _settingsContainer = settingsContainer;
            _applicationConfigurationLoadUnit = applicationConfigurationLoadUnit;
            _configContainer = configContainer;
        }

        public async UniTask StartAsync(CancellationToken cancellation = new())
        {
            await _loadingService.BeginLoading(_settingsContainer);
            await _loadingService.BeginLoading(_configContainer);
            await _loadingService.BeginLoading(_applicationConfigurationLoadUnit);

            UniRxUtils.Initialize();
            await _sceneManager.LoadScene(RuntimeConstants.Scenes.Menu);
        }


    }
}