using Chess3D.Runtime.Core.UI.UnityGUI.ClockUI;
using VContainer;

namespace Chess3D.Runtime.Core.Logic
{
    [UnityEngine.Scripting.Preserve]
    public sealed class ClockPanelFactory
    {
        private ClockCanvas _cachedInstance;

        private readonly IObjectResolver _resolver;
        private readonly ISettingsContainer settingsContainer;

        public ClockPanelFactory(IObjectResolver resolver, ISettingsContainer settingsContainer)
        {
            _resolver = resolver;
            this.settingsContainer = settingsContainer;
        }

        public ClockCanvas Create()
        {
            return _cachedInstance ??= _resolver.Resolve<ClockCanvas>(settingsContainer.S.GameSettings.PlayerColor);
        }
    }
}