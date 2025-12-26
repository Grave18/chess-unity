using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using VContainer.Unity;

namespace Chess3D.Runtime.Online
{
    [Preserve]
    public class OnlineFlow : IAsyncStartable
    {
        private readonly ConnectionStarter _connectionStarter;
        private readonly PlayersDataExchanger _playersDataExchanger;

        public OnlineFlow(ConnectionStarter connectionStarter, PlayersDataExchanger playersDataExchanger)
        {
            _connectionStarter = connectionStarter;
            _playersDataExchanger = playersDataExchanger;
        }

        public async UniTask StartAsync(CancellationToken cancellation = new())
        {
            await _connectionStarter.Load();
            await _playersDataExchanger.Load();
        }
    }
}