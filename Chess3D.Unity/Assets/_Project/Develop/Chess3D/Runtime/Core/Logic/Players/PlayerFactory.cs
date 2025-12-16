using Chess3D.Runtime.Bootstrap.Settings;
using VContainer;

namespace Chess3D.Runtime.Core.Logic.Players
{
    [UnityEngine.Scripting.Preserve]
    public class PlayerFactory
    {
        private readonly IObjectResolver _resolver;
        private readonly ISettingsContainer settingsContainer;

        public PlayerFactory(IObjectResolver resolver, ISettingsContainer settingsContainer)
        {
            _resolver = resolver;
            this.settingsContainer = settingsContainer;
        }

        public IPlayer Create(PlayerType playerType)
        {
            var input = _resolver.Resolve<IInputHandler>(playerType);
            input.Load();
            return new PlayerOffline(input);
        }
    }
}