using System.Threading;
using Chess3D.Runtime.Bootstrap.Settings;
using Cysharp.Threading.Tasks;
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

        public async UniTask<IPlayer> Create(PlayerType playerType, CancellationToken cancellationToken = default)
        {
            var input = _resolver.Resolve<IInputHandler>(playerType);
            await input.Load(cancellationToken);

            return new Player(input);
        }
    }
}