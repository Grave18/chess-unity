using Chess3D.Runtime.Bootstrap.Settings;
using PurrNet;
using VContainer;

namespace Chess3D.Runtime.Core.Logic.Players
{
    [UnityEngine.Scripting.Preserve]
    public sealed class PlayerFactory
    {
        private readonly IObjectResolver _resolver;

        public PlayerFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public IPlayer Create(PlayerType playerType)
        {
            var input = _resolver.Resolve<IInputHandler>(playerType);
            input.Load();

            return new PlayerOffline(input);
        }
    }
}