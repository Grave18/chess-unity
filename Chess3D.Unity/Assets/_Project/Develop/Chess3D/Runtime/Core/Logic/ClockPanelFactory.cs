using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.MainCamera;
using Chess3D.Runtime.Core.UI.UnityGUI.ClockUi;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.Logic
{
    [UnityEngine.Scripting.Preserve]
    public class ClockPanelFactory
    {
        private ClockPanelCanvas _cachedInstance;

        private readonly IObjectResolver _resolver;
        private readonly ISettingsService _settingsService;

        public ClockPanelFactory(IObjectResolver resolver, ISettingsService settingsService)
        {
            _resolver = resolver;
            _settingsService = settingsService;
        }

        public ClockPanelCanvas Create()
        {
            if (_cachedInstance)
            {
                return _cachedInstance;
            }

            var game = _resolver.Resolve<Game>();
            var clock = _resolver.Resolve<IClock>();
            var uiCamera = _resolver.Resolve<Camera>(CameraKeys.Ui);
            var coreEvents = _resolver.Resolve<CoreEvents>();
            _cachedInstance = _resolver.Resolve<ClockPanelCanvas>(_settingsService.S.GameSettings.PlayerColor);
            _cachedInstance.Construct(game, coreEvents, clock, uiCamera);

            return _cachedInstance;
        }
    }
}