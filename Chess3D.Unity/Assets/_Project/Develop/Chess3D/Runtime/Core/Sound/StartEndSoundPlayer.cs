using System;
using UniRx;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Core.Sound
{
    [Preserve]
    public class StartEndSoundPlayer : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();

        public StartEndSoundPlayer(CoreEvents coreEvents, EffectsPlayer effectsPlayer)
        {
            coreEvents.OnStart.Subscribe(_ => effectsPlayer.PlayGameStartSound()).AddTo(_disposables);
            coreEvents.OnEnd.Subscribe(_ => effectsPlayer.PlayGameEndSound()).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
