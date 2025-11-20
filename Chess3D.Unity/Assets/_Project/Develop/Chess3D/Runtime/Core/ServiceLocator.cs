using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core
{
    public static class ServiceLocator
    {
        private static IObjectResolver Resolver { get; set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            Resolver = null;
        }

        public static void Initialize(IObjectResolver resolver)
        {
            Resolver = resolver;
        }

        public static T Resolve<T>()
        {
            return Resolver.Resolve<T>();
        }
    }
}