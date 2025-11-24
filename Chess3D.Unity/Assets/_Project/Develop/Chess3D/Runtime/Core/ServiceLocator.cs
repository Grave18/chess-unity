using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core
{
    public static class ServiceLocator
    {
        private static IObjectResolver _resolver;
        public static bool IsValid => _resolver != null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            _resolver = null;
        }

        public static void Initialize(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public static T Resolve<T>()
        {
            return _resolver.Resolve<T>();
        }

        public static async UniTask<T> ResolveAsync<T>()
        {
            await UniTask.WaitUntil(() => IsValid);
            var ret = _resolver.Resolve<T>();

            return ret;
        }
    }
}