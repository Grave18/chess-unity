using System.Threading;
using Cysharp.Threading.Tasks;

namespace Chess3D.Runtime.Core.Logic.Players
{
    public interface IPlayer
    {
        UniTask StartPlayer(CancellationToken cancellationToken = default);
        void UpdatePlayer();
        void StopPlayer();
    }
}