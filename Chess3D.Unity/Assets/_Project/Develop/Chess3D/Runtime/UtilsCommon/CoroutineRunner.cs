using System;
using System.Collections;
using UnityEngine;

namespace Chess3D.Runtime.UtilsCommon
{
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;


        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    var coroutineRunnerGo = new GameObject("CoroutineRunner");
                    _instance = coroutineRunnerGo.AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(coroutineRunnerGo);
                }

                return _instance;
            }
        }

        public static IEnumerator Delay(float sec)
        {
            yield return new WaitForSeconds(sec);
        }

        public static Coroutine Run(IEnumerator coroutine)
        {
            return Instance.StartCoroutine(coroutine);
        }

        public static Coroutine RunWithCallback(IEnumerator coroutine, Action continuation)
        {
            return Run(CoroutineWithCallback(coroutine, continuation));
        }

        private static IEnumerator CoroutineWithCallback(IEnumerator coroutine, Action continuation)
        {
            yield return coroutine;
            continuation?.Invoke();
        }

        public static void Stop(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                Instance.StopCoroutine(coroutine);
            }
        }
    }
}