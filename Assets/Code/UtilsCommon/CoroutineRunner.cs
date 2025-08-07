using System.Collections;
using UnityEngine;

namespace UtilsCommon
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

        public static Coroutine Run(IEnumerator coroutine)
        {
            return Instance.StartCoroutine(coroutine);
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