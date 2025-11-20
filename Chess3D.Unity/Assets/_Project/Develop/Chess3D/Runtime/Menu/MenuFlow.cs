using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using VContainer.Unity;

namespace Chess3D.Runtime.Menu
{
    [Preserve]
    public class MenuFlow : IAsyncStartable
    {
        public async  UniTask StartAsync(CancellationToken cancellation = new())
        {
        }
    }
}