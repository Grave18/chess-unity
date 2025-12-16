using System.Threading;
using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer.Unity;

namespace Chess3D.Runtime.Bootstrap
{
    [Preserve]
    public class BootstrapFlow : IAsyncStartable
    {
        private readonly LoadingService _loadingService;
        private readonly SceneManager _sceneManager;
        private readonly ISettingsContainer settingsContainer;
        private readonly ApplicationConfigurationLoadUnit _applicationConfigurationLoadUnit;

        public BootstrapFlow(LoadingService loadingService, SceneManager sceneManager, ISettingsContainer settingsContainer,
            ApplicationConfigurationLoadUnit applicationConfigurationLoadUnit)
        {
            _loadingService = loadingService;
            _sceneManager = sceneManager;
            this.settingsContainer = settingsContainer;
            _applicationConfigurationLoadUnit = applicationConfigurationLoadUnit;
        }

        public async UniTask StartAsync(CancellationToken cancellation = new())
        {
            await _loadingService.BeginLoading(settingsContainer);
            await _loadingService.BeginLoading(_applicationConfigurationLoadUnit);

            UniRxUtils.Initialize();
            await _sceneManager.LoadScene(RuntimeConstants.Scenes.Menu);
        }


    }
}