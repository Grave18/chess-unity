using System;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Core.Sound
{
    [Preserve]
    public class StartEndSoundPlayer : IDisposable
    {
        private readonly CoreEvents _coreEvents;
        private readonly EffectsPlayer _effectsPlayer;

        public StartEndSoundPlayer(CoreEvents coreEvents, EffectsPlayer effectsPlayer)
        {
            _coreEvents = coreEvents;
            _effectsPlayer = effectsPlayer;

            _coreEvents.OnStart += _effectsPlayer.PlayGameStartSound;
            _coreEvents.OnEnd += _effectsPlayer.PlayGameEndSound;
        }

        public void Dispose()
        {
            if (_coreEvents is null)
            {
                return;
            }

            _coreEvents.OnStart -= _effectsPlayer.PlayGameStartSound;
            _coreEvents.OnEnd -= _effectsPlayer.PlayGameEndSound;
        }
    }
}
