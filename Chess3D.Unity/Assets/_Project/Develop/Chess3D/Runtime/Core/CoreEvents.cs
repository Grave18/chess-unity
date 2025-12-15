using UniRx;
using UnityEngine.Events;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Core
{
    [Preserve]
    public class CoreEvents
    {
        // Events and invokers
        public readonly Subject<Unit> OnWarmup = new();
        public readonly Subject<Unit> OnStart = new();
        public readonly Subject<Unit> OnIdle = new();
        public readonly Subject<Unit> OnEnd = new();
        public readonly Subject<Unit> OnEndMove = new();
        public readonly Subject<Unit> OnPlay = new();
        public readonly Subject<Unit> OnPause = new();

        public void FireStart() => OnStart.OnNext(Unit.Default);
        public void FireIdle() => OnIdle.OnNext(Unit.Default);
        public void FireEnd() => OnEnd.OnNext(Unit.Default);
        public void FireEndMove() => OnEndMove.OnNext(Unit.Default);
        public void FirePlay() => OnPlay.OnNext(Unit.Default);
        public void FirePause() => OnPause.OnNext(Unit.Default);
    }
}