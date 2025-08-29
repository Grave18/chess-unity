using System.Collections;
using UnityEngine;

namespace UtilsCommon
{
    public static class CoroutineExtension
    {
        public static Coroutine RunCoroutine(this IEnumerator coroutine)
        {
            return CoroutineRunner.Run(coroutine);
        }

        public static void Stop(this Coroutine coroutine)
        {
            CoroutineRunner.Stop(coroutine);
        }
    }
}