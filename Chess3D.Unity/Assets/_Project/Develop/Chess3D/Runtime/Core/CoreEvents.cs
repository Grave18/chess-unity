using UnityEngine.Events;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Core
{
    [Preserve]
    public class CoreEvents
    {
        // Events and invokers
        public event UnityAction OnWarmup;
        public event UnityAction OnStart;
        public event UnityAction OnIdle;
        public event UnityAction OnEnd;
        public event UnityAction OnEndMove;
        public event UnityAction OnPlay;
        public event UnityAction OnPause;

        public void FireWarmup() => OnWarmup?.Invoke();
        public void FireStart() => OnStart?.Invoke();
        public void FireIdle() => OnIdle?.Invoke();
        public void FireEnd() => OnEnd?.Invoke();
        public void FireEndMove() => OnEndMove?.Invoke();
        public void FirePlay() => OnPlay?.Invoke();
        public void FirePause() => OnPause?.Invoke();
    }
}