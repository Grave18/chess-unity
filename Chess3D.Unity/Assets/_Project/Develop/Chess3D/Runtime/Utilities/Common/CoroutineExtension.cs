using System;
using System.Collections;
using UnityEngine;

namespace Chess3D.Runtime.Utilities.Common
{
    public static class CoroutineExtension
    {
        public static Coroutine RunCoroutine(this IEnumerator coroutine)
        {
            return CoroutineRunner.Run(coroutine);
        }

        public static Coroutine RunCoroutineWithCallback(this IEnumerator coroutine, Action continuation)
        {
            return CoroutineRunner.RunWithCallback(coroutine, continuation);
        }

        public static void Stop(this Coroutine coroutine)
        {
            CoroutineRunner.Stop(coroutine);
        }
    }
}