using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.UI.UnityGUI.ClockUi;
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
            return _cachedInstance ??= _resolver.Resolve<ClockPanelCanvas>(_settingsService.S.GameSettings.PlayerColor);
        }
    }
}