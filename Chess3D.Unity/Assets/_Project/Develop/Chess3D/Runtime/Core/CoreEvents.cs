using UniRx;
using UnityEngine.Events;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Core
{
    [Preserve]
    public class CoreEvents
    {
        public readonly Subject<Unit> OnWarmup = new();
        public readonly Subject<Unit> OnStart = new();
        public readonly Subject<Unit> OnIdle = new();
        public readonly Subject<Unit> OnEnd = new();
        public readonly Subject<Unit> OnEndMove = new();
        public readonly Subject<Unit> OnPlay = new();
        public readonly Subject<Unit> OnPause = new();
        public readonly Subject<UnityAction<string>> RequestPromotion = new();

    }
}