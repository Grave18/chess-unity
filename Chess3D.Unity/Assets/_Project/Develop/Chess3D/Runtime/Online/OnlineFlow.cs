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
        private readonly OnlineSceneManager _onlineSceneManager;
        private readonly WaitCanvas _waitCanvas;

        public OnlineFlow(ConnectionStarter connectionStarter, PlayersDataExchanger playersDataExchanger,
            OnlineSceneManager onlineSceneManager, WaitCanvas waitCanvas)
        {
            _connectionStarter = connectionStarter;
            _playersDataExchanger = playersDataExchanger;
            _onlineSceneManager = onlineSceneManager;
            _waitCanvas = waitCanvas;
        }

        public async UniTask StartAsync(CancellationToken cancellation = new())
        {
            await _connectionStarter.Load();
            await _playersDataExchanger.Load();

            _waitCanvas.Hide();
            _onlineSceneManager.LoadCorePrefab();
        }
    }
}