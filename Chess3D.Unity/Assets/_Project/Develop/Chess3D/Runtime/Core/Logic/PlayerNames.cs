using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Core.Logic
{
    [Preserve]
    public class PlayerNames : ILoadUnit
    {
        public Subject<string> WhitePlayerName { get; } = new();
        public Subject<string> BlackPlayerName { get; } = new();

        private readonly ISettingsContainer _settingsContainer;

        public PlayerNames(ISettingsContainer settingsContainer)
        {
            _settingsContainer = settingsContainer;
        }

        public UniTask Load()
        {
            WhitePlayerName.OnNext(_settingsContainer.GameSettings.PlayerWhite.Name);
            BlackPlayerName.OnNext(_settingsContainer.GameSettings.PlayerBlack.Name);

            return UniTask.CompletedTask;
        }
    }
}