using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Online;
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

        public IPlayer Create(PlayerType playerType, ConnectionType connectionType, PieceColor pieceColor)
        {
            var input = _resolver.Resolve<IInputHandler>(playerType);
            input.Load();

            if (connectionType == ConnectionType.Online)
            {
                var player = _resolver.Resolve<PlayerOnline>(pieceColor);
                player.Construct(input);
                return player;
            }

            return new PlayerOffline(input);
        }
    }
}