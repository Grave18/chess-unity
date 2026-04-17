using System.Threading;
using Cysharp.Threading.Tasks;

namespace Chess3D.Runtime.Core.Logic.Players
{
    public interface IInputHandler
    {
        public UniTask Load(CancellationToken cancellationToken = default);
        public UniTask StartInput();
        void UpdateInput();
        public void StopInput();
    }
}