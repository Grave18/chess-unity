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
        private readonly ISettingsContainer settingsContainer;

        public ClockPanelFactory(IObjectResolver resolver, ISettingsContainer settingsContainer)
        {
            _resolver = resolver;
            this.settingsContainer = settingsContainer;
        }

        public ClockPanelCanvas Create()
        {
            return _cachedInstance ??= _resolver.Resolve<ClockPanelCanvas>(settingsContainer.S.GameSettings.PlayerColor);
        }
    }
}